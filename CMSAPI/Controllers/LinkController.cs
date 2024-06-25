using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }       

        public LinkController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper                    
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;          
        }

        #region Links

        [Authorize]
        [HttpGet]
        [Route("GetLinks")]
        public async Task<IActionResult> GetLinks(int webLangId = 1)
        {
            string[] includes = { "Layout", "Page", "LinkType" };
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var LinksList = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == webLangId && layoutIds.Contains(t.LayoutId) && t.Active == true, false, includes).ToListAsync();


            if (LinksList.Count > 0)
            {
                var linksDto = _mapper.Map<IEnumerable<LinkListDto>?>(LinksList);
                return Ok(linksDto);
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [Route("GetLinksAsync")]
        public async Task<IActionResult> GetLinksAsync([FromBody] LinkFilterParameters parameter)
        {
            //var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            //var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            //var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            //List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var link = await _unitOfWork.Links.GetLinkAsync(parameter);
            var linksDto = _mapper.Map<IEnumerable<LinkListDto>?>(link);

            return Ok(new
            {
                data = linksDto,
                total = link.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpGet("GetLinkById")]
        public async Task<IActionResult> GetLinkById(int id, int webLangId)
        {
            var data = await _unitOfWork.Links.GetLinkById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<LinkDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateLink")]
        public async Task<IActionResult> CreateLink([FromBody] LinkDto model)
        {
            if (ModelState.IsValid)
            {              
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int linkId = _unitOfWork.Links.GetMaxPK(a => a.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = linkId;
                if (model.WebMultiLang)
                {   
                    foreach(var item in langList)
                    {
                        var multilinkEntity = _mapper.Map<Link>(model);
                        multilinkEntity.LanguageId = item.Id;
                        multilinkEntity.CreatedBy = userinId;
                        multilinkEntity.Created = DateTime.Now;
                        await _unitOfWork.Links.Create(multilinkEntity);

                        await _unitOfWork.Links.Commit();
                    }
                    
                    return StatusCode(StatusCodes.Status201Created);
                }
               
                var linkEntity = _mapper.Map<Link>(model);
                linkEntity.CreatedBy = userinId;
                linkEntity.Created = DateTime.Now;
                await _unitOfWork.Links.Create(linkEntity);

                await _unitOfWork.Links.Commit();

                return StatusCode(StatusCodes.Status201Created, linkEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut("UpdateLink")]
        public async Task<IActionResult> UpdateLink([FromBody] LinkDto model)
        {
            var linkEntity = await _unitOfWork.Links.GetLinkById(model.Id, model.LanguageId);

            if (linkEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, linkEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            linkEntity.ModifiedBy = userinId;
            linkEntity.Modified = DateTime.Now;
            _unitOfWork.Links.Update(linkEntity);
            await _unitOfWork.Links.Commit();

            var menuToReturn = _mapper.Map<LinkDto>(linkEntity);

            return Ok(menuToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteLink")]
        public async Task<IActionResult> DeleteLink(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var link = await _unitOfWork.Links.GetLinkById(id, webLangId);

            if (link == null)
            {
                return NotFound();
            }
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multilinkEntity = await _unitOfWork.Links.GetLinkById(id, item.Id);
                    if (multilinkEntity != null)
                    {
                        _unitOfWork.Links.Delete(multilinkEntity);
                        await _unitOfWork.Links.Commit();
                    }
                }
            }
            else
            {
                _unitOfWork.Links.Delete(link);

                await _unitOfWork.Links.Commit();
            }
            return Ok();
        }


        [Authorize]
        [HttpPost]
        [Route("OrderLinks")]
        public async Task<ActionResult> OrderLinks(List<UpdateLinkListDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var links = await _unitOfWork.Links.GetLinkById(item.Id, item.LanguageId);
                    if (links != null)
                    {
                        links.OrderNo = item.OrderNo;
                        _unitOfWork.Links.Update(links);
                        await _unitOfWork.Links.Commit();
                    }
                }
                return Ok();
            }

            return NotFound();
        }



        #endregion

        #region LinkType

        [Authorize]
        [HttpPost]
        [Route("GetLinksTypesAsync")]
        public async Task<IActionResult> GetLinksTypesAsync([FromBody] LinkTypeFilterParameters parameter)
        {
            var linkTypes = await _unitOfWork.LinkTypes.GetLinksTypesAsync(parameter);
            var linksTypesDto = _mapper.Map<IEnumerable<LinkTypeListDto>?>(linkTypes);

            return Ok(new
            {
                data = linksTypesDto,
                total = linkTypes.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpGet("GetLinkTypeById")]
        public async Task<IActionResult> GetLinkTypeById(int id, int webLangId)
        {
            var data = await _unitOfWork.LinkTypes.GetLinksTypesById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<LinkTypeDto>(data);

                return Ok(dataDto);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("CreateLinkType")]
        public async Task<IActionResult> CreateLinkType([FromBody] LinkTypeDto model)
        {
            if (ModelState.IsValid)
            {
                int linktypeId =  _unitOfWork.LinkTypes.GetMaxPK(i => i.LinkTypeId);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.LinkTypeId = linktypeId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var multilinkEntity = _mapper.Map<LinkType>(model);
                        multilinkEntity.LanguageId = item.Id;

                        await _unitOfWork.LinkTypes.Create(multilinkEntity);

                        await _unitOfWork.LinkTypes.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created);
                }

                var linktypeEntity = _mapper.Map<LinkType>(model);
              
                await _unitOfWork.LinkTypes.Create(linktypeEntity);

                await _unitOfWork.LinkTypes.Commit();

                return StatusCode(StatusCodes.Status201Created, linktypeEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [Authorize]
        [HttpPut("UpdateLinkType")]
        public async Task<IActionResult> UpdateLinkType([FromBody] LinkTypeDto model)
        {
            var linktypeEntity = await _unitOfWork.LinkTypes.GetLinksTypesById(model.LinkTypeId, model.LanguageId);

            if (linktypeEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, linktypeEntity);

            _unitOfWork.LinkTypes.Update(linktypeEntity);

            await _unitOfWork.LinkTypes.Commit();

            var menuToReturn = _mapper.Map<LinkTypeDto>(linktypeEntity);

            return Ok(menuToReturn);
        }


        [Authorize]
        [HttpDelete("DeleteLinkType")]
        public async Task<IActionResult> DeleteLinkType(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var linktypes = await _unitOfWork.LinkTypes.GetLinksTypesById(id, webLangId);

            if (linktypes == null)
            {
                return NotFound();
            }
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multilinkTypesEntity = await _unitOfWork.LinkTypes.GetLinksTypesById(id, item.Id);
                    if (multilinkTypesEntity != null)
                    {
                        _unitOfWork.LinkTypes.Delete(multilinkTypesEntity);
                        await _unitOfWork.LinkTypes.Commit();
                    }
                }
            }
            else
            {
                _unitOfWork.LinkTypes.Delete(linktypes);

                await _unitOfWork.LinkTypes.Commit();
            }
            return Ok();
        }
        #endregion

    }
}
