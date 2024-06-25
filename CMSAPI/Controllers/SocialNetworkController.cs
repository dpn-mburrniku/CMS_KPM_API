using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialNetworkController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }       

        public SocialNetworkController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper                    
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;          
        }       


        [Authorize]
        [HttpGet]
        [Route("GetSocialNetwork")]
        public async Task<IActionResult> GetSocialNetwork(int webLangId = 1)
        {
            string[] includes = { "Layout", "ComponentLocation" };
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var SocialNeworksList = await _unitOfWork.SocialNetwork.FindByCondition(t => t.LanguageId == webLangId && t.Active == true 
            && layoutIds.Contains(t.LayoutId), false, includes).ToListAsync();

            if (SocialNeworksList.Count > 0)
            {
                var socialnetworksDto = _mapper.Map<IEnumerable<SocialNetworkListDto>?>(SocialNeworksList);
                return Ok(socialnetworksDto);
            }

            return NotFound();
        }



        [Authorize]
        [HttpPost]
        [Route("GetSocialNetworkAsync")]
        public async Task<IActionResult> GetSocialNetworkAsync([FromBody] SocialNetworkFilterParameters parameter)
        {
            //var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            //var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            //var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            //List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var socialnetwork = await _unitOfWork.SocialNetwork.GetSocialNetworkAsync(parameter);
            var socialnetworksDto = _mapper.Map<IEnumerable<SocialNetworkListDto>?>(socialnetwork);

            return Ok(new
            {
                data = socialnetworksDto,
                total = socialnetwork.MetaData.TotalCount
            });
        }


        [Authorize]
        [HttpGet("GetSocialNetworkById")]
        public async Task<IActionResult> GetSocialNetworkById(int id, int webLangId)
        {
            var data = await _unitOfWork.SocialNetwork.GetSocialNetworkById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<SocialNetworkDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateSocialNetwork")]
        public async Task<IActionResult> CreateSocialNetwork([FromBody] SocialNetworkDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int SocialNetworkId = _unitOfWork.SocialNetwork.GetMaxPK(i => i.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = SocialNetworkId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var SocialNetworksList = await _unitOfWork.SocialNetwork.FindByCondition(t => t.LanguageId == item.Id
                                                                                   && t.LayoutId == model.LayoutId && t.ComponentLocationId == model.ComponentLocationId, false, null).ToListAsync();
                        if (SocialNetworksList.Where(t => t.OrderNo == 1).Count() > 0)
                        {
                            var socialnetworksToReturn = _mapper.Map<List<UpdateSocialNetworkListDto>>(SocialNetworksList);
                            await UpdateSocialNetworkOrderBack(socialnetworksToReturn);
                        }
                        var multiSocialNetworkEntity = _mapper.Map<SocialNetwork>(model);
                        multiSocialNetworkEntity.LanguageId = item.Id;
                        multiSocialNetworkEntity.CreatedBy = userinId;
                        multiSocialNetworkEntity.Created = DateTime.Now;
                        multiSocialNetworkEntity.OrderNo = 1;
                        await _unitOfWork.SocialNetwork.Create(multiSocialNetworkEntity);

                        await _unitOfWork.SocialNetwork.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created);
                }

                var SocialNetworksListOneLang = await _unitOfWork.SocialNetwork.FindByCondition(t => t.LanguageId == model.LanguageId
                                                                                 && t.LayoutId == model.LayoutId && t.ComponentLocationId == model.ComponentLocationId, false, null).ToListAsync();
                if (SocialNetworksListOneLang.Where(t => t.OrderNo == 1).Count() > 0)
                {
                    var socialnetworksToReturn = _mapper.Map<List<UpdateSocialNetworkListDto>>(SocialNetworksListOneLang);
                    await UpdateSocialNetworkOrderBack(socialnetworksToReturn);
                }
                var SocialNetworkEntity = _mapper.Map<SocialNetwork>(model);
                SocialNetworkEntity.CreatedBy = userinId;
                SocialNetworkEntity.Created = DateTime.Now;
                SocialNetworkEntity.OrderNo = 1;
                await _unitOfWork.SocialNetwork.Create(SocialNetworkEntity);

                await _unitOfWork.SocialNetwork.Commit();

                return StatusCode(StatusCodes.Status201Created, SocialNetworkEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut("UpdateSocialNetwork")]
        public async Task<IActionResult> UpdateSocialNetwork([FromBody] SocialNetworkDto model)
        {
            var socialnetworkEntity = await _unitOfWork.SocialNetwork.GetSocialNetworkById(model.Id, model.LanguageId);

            if (socialnetworkEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, socialnetworkEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            socialnetworkEntity.ModifiedBy = userinId;
            socialnetworkEntity.Modified = DateTime.Now;            

            _unitOfWork.SocialNetwork.Update(socialnetworkEntity);
            await _unitOfWork.SocialNetwork.Commit();

            var menuToReturn = _mapper.Map<SocialNetworkDto>(socialnetworkEntity);
            return Ok(menuToReturn);

        }


        [Authorize]
        [HttpDelete("DeleteSocialNetwork")]
        public async Task<IActionResult> DeleteSocialNetwork(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var socialnetworkId = await _unitOfWork.SocialNetwork.GetSocialNetworkById(id, webLangId);

            if (socialnetworkId == null)
            {
                return NotFound();
            }

            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multiSocialNetworkEntity = await _unitOfWork.SocialNetwork.GetSocialNetworkById(id, item.Id);
                    if (multiSocialNetworkEntity != null)
                    {
                        _unitOfWork.SocialNetwork.Delete(multiSocialNetworkEntity);
                        await _unitOfWork.SocialNetwork.Commit();
                    }
                }

                return Ok();
            }

            _unitOfWork.SocialNetwork.Delete(socialnetworkId);
            await _unitOfWork.SocialNetwork.Commit();
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("OrderSocialNetwork")]
        public async Task<ActionResult> OrderSocialNetwork(List<UpdateSocialNetworkListDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var socialnetworks = await _unitOfWork.SocialNetwork.GetSocialNetworkById(item.Id, item.LanguageId);
                    if (socialnetworks != null)
                    {
                        socialnetworks.OrderNo = item.OrderNo;
                        _unitOfWork.SocialNetwork.Update(socialnetworks);
                        await _unitOfWork.SocialNetwork.Commit();
                    }
                }
                return Ok();
            }

            return NotFound();
        }

        private async Task<bool> UpdateSocialNetworkOrderBack(List<UpdateSocialNetworkListDto>? model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var socialnetworks = await _unitOfWork.SocialNetwork.GetSocialNetworkById(item.Id, item.LanguageId);
                    if (socialnetworks != null)
                    {
                        socialnetworks.OrderNo = item.OrderNo + 1;
                        _unitOfWork.SocialNetwork.Update(socialnetworks);
                    }
                }
                await _unitOfWork.SocialNetwork.Commit();
                return true;
            }
            return false;
        }
    }
}
