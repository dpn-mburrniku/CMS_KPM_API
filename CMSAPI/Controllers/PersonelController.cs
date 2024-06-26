using AutoMapper;
using CMS.API.InternalServices;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Abp.Linq.Expressions;
using System.Runtime.InteropServices;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonelController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        private IFileRepository _fileRepository;
        public IMapper _mapper { get; }
        public PersonelController(IUnitOfWork unitOfWork
                            , IMapper mapper
                            , IFileRepository fileRepository) {
            
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileRepository = fileRepository;
        }


        [Authorize]
        [HttpGet]
        [Route("GetPersonels")]
        public async Task<IActionResult> GetPersonel(int webLangId = 1)
        {
            string[] includes = { "Layout", "Page", "Media" };
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            List<int> layoutIds = layouts.Select(x => x.Id).ToList(); 
            
            var personelQuery = PredicateBuilder.False<Personel>();

            foreach (var id in layoutIds)
            {
                personelQuery = personelQuery.Or(t => t.LayoutId == id);
            }
            personelQuery = personelQuery.And(t => t.LanguageId == webLangId);
            personelQuery = personelQuery.And(t => t.Active == true);

            var PersonelsList = await _unitOfWork.Personeli.FindByCondition(personelQuery, false, includes).ToListAsync();

            if (PersonelsList.Count > 0)
            {
                var personelsDto = _mapper.Map<IEnumerable<PersonelListDto>?>(PersonelsList);
                return Ok(personelsDto);
            }

            return NotFound();
        }


        [Authorize]
        [HttpPost]
        [Route("GetPersonelsAsync")]
        public async Task<IActionResult> GetPersonelsAsync([FromBody] PersonelFilterParameters parameter)
        {
            //var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            //var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            //var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            //List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var personel = await _unitOfWork.Personeli.GetPersonelAsync(parameter);
            var personelsDto = _mapper.Map<IEnumerable<PersonelListDto>?>(personel);

            return Ok(new
            {
                data = personelsDto,
                total = personel.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpPost]
        [Route("CreatePersonel")]
        public async Task<IActionResult> CreatePersonel([FromForm] PersonelDto model)
        {
            if(ModelState.IsValid)
            {               
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int personelId = _unitOfWork.Personeli.GetMaxPK(i => i.Id);
                var langlist = await _unitOfWork.BaseConfig.GetLangList();
                var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
                
                model.Id = personelId;

                if (model.WebMultiLang)
                {
                    var mediaId = 0;
                    if (model.Image != null)
                    {                        
                        int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                        maxLength = maxLength * 1048576;
                        if (maxLength >= model.Image.Length)
                        {
                            var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                            string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

                            Medium media = new Medium
                            {
                                MediaExCategoryId = 1,
                                Name = model.Image.FileName.Replace(fileEx, ""),
                                FileName = Guid.Parse(filename),
                                FileNameMedium = filename + "_medium",
                                FileNameSmall = filename + "_small",
                                FileEx = fileEx,
                                IsOtherSource = false,
                                CreatedBy = userinId,
                                Created = DateTime.Now
                            };
                            await _unitOfWork.Media.Create(media);
                            await _unitOfWork.Media.Commit();
                            mediaId = media.Id;
                        }
                    }
                    foreach (var item in langlist)
                    {
                        var PersonelList = await _unitOfWork.Personeli.FindByCondition(t => t.LanguageId == item.Id
                                                                                 && t.LayoutId == model.LayoutId && t.PageId == model.PageId, false, null).ToListAsync();
                        if (PersonelList.Where(t => t.OrderNo == 1).Count() > 0)
                        {
                            var personelToReturn = _mapper.Map<List<UpdateOrderPersonelDto>>(PersonelList);
                            await UpdatePersonelOrderBack(personelToReturn);
                        }
                        var multiPersonelEntity = _mapper.Map<Personel>(model);   
                        multiPersonelEntity.LanguageId = item.Id;
                        multiPersonelEntity.CreatedBy = userinId;
                        multiPersonelEntity.Created = DateTime.Now;
                        multiPersonelEntity.OrderNo = 1;
                        if (mediaId != 0)
                        {
                            multiPersonelEntity.MediaId = mediaId;
                        }
                        if (!string.IsNullOrEmpty(model.BirthDateStr))
                        {
                            var dtBirthDate = _unitOfWork.BaseConfig.StringToDate(model.BirthDateStr);
                            multiPersonelEntity.BirthDate = dtBirthDate;
                        }
                        else
                        {
                            multiPersonelEntity.BirthDate = null;
                        }

                        await _unitOfWork.Personeli.Create(multiPersonelEntity);
                        await _unitOfWork.Personeli.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created, new { Id = personelId });
                }

                var PerosnelListOneLang = await _unitOfWork.Personeli.FindByCondition(t => t.LanguageId == model.LanguageId
                                                                           && t.LayoutId == model.LayoutId && t.PageId == model.PageId, false, null).ToListAsync();
                if (PerosnelListOneLang.Where(t => t.OrderNo == 1).Count() > 0)
                {
                    var personelToReturnOneLang = _mapper.Map<List<UpdateOrderPersonelDto>>(PerosnelListOneLang);
                    await UpdatePersonelOrderBack(personelToReturnOneLang);
                }
                var PersonelEntity = _mapper.Map<Personel>(model);
                PersonelEntity.CreatedBy = userinId;
                PersonelEntity.Created = DateTime.Now;
                PersonelEntity.OrderNo = 1;
                if (!string.IsNullOrEmpty(model.BirthDateStr))
                {
                    var dtBirthDate = _unitOfWork.BaseConfig.StringToDate(model.BirthDateStr);
                    PersonelEntity.BirthDate = dtBirthDate;
                }
                else
                {
                    PersonelEntity.BirthDate = null;
                }
                if (model.Image != null)
                {
                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.Image.Length)
                    {
                        var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                        string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

                        Medium media = new Medium
                        {
                            MediaExCategoryId = 1,
                            Name = model.Image.FileName.Replace(fileEx, ""),
                            FileName = Guid.Parse(filename),
                            FileNameMedium = filename + "_medium",
                            FileNameSmall = filename + "_small",
                            FileEx = fileEx,
                            IsOtherSource = false,
                            CreatedBy = userinId,
                            Created = DateTime.Now
                        };
                        await _unitOfWork.Media.Create(media);
                        await _unitOfWork.Media.Commit();                       
                        if (media.Id != 0)
                        {
                            PersonelEntity.MediaId = media.Id;
                        }
                    }
                }
                
                await _unitOfWork.Personeli.Create(PersonelEntity);

                await _unitOfWork.Personeli.Commit();

                return StatusCode(StatusCodes.Status201Created, new { Id = personelId });


            } else
            {
                return BadRequest(ModelState);
            }

        }

        [Authorize]
        [HttpGet("GetPersonelById")]
        public async Task<IActionResult> GetPersonelById(int id, int webLangId)
        {
            var data = await _unitOfWork.Personeli.GetPersonelById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                
                var dataDto = _mapper.Map<PersonelGetByIdDto>(data);
                if (data.BirthDate != null)
                {

                    dataDto.BirthDateStr  = data.BirthDate.Value.ToString("dd/MM/yyyy");
                }
                return Ok(dataDto);
            }
        }


        [Authorize]
        [HttpPut("UpdatePersonel")]
        public async Task<IActionResult> UpdatePersonel([FromForm] PersonelDto model)
        {
            var personelEntity = await _unitOfWork.Personeli.GetPersonelById(model.Id, model.LanguageId);
            var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            if (personelEntity == null)
            {
                int? pageId = null;

                if (model.PageId != null)
                {
                    var page = await _unitOfWork.Pages.GetPageById((int)model.PageId, model.LanguageId);
                    if (page != null)
                        pageId = page.Id;
                }
                personelEntity = _mapper.Map<Personel>(model);
                personelEntity.PageId = pageId;
                personelEntity.CreatedBy = userinId;
                personelEntity.Created = DateTime.Now;
                if (!string.IsNullOrEmpty(model.BirthDateStr))
                {
                    var dtBirthDate = _unitOfWork.BaseConfig.StringToDate(model.BirthDateStr);
                    personelEntity.BirthDate = dtBirthDate;
                }
                else
                {
                    personelEntity.BirthDate = null;
                }

                if (model.Image != null)
                {
                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.Image.Length)
                    {
                        var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                        string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

                        Medium media = new Medium
                        {
                            MediaExCategoryId = 1,
                            Name = model.Image.FileName.Replace(fileEx, ""),
                            FileName = Guid.Parse(filename),
                            FileNameMedium = filename + "_medium",
                            FileNameSmall = filename + "_small",
                            FileEx = fileEx,
                            IsOtherSource = false,
                            CreatedBy = userinId,
                            Created = DateTime.Now
                        };
                        await _unitOfWork.Media.Create(media);
                        await _unitOfWork.Media.Commit();
                        if (media.Id != 0)
                        {
                            personelEntity.MediaId = media.Id;
                        }
                    }
                }

                await _unitOfWork.Personeli.Create(personelEntity);
            }
            else
            {
                _mapper.Map(model, personelEntity);
               
                #region Foto
                if (model.Image != null)
                {
                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.Image.Length)
                    {
                        var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                        string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

                        Medium media = new Medium
                        {
                            MediaExCategoryId = 1,
                            Name = model.Image.FileName.Replace(fileEx, ""),
                            FileName = Guid.Parse(filename),
                            FileNameMedium = filename + "_medium",
                            FileNameSmall = filename + "_small",
                            FileEx = fileEx,
                            IsOtherSource = false,
                            CreatedBy = userinId,
                            Created = DateTime.Now
                        };
                        await _unitOfWork.Media.Create(media);
                        await _unitOfWork.Media.Commit();
                        personelEntity.MediaId = media.Id;
                    }
                }
                #endregion

                personelEntity.ModifiedBy = userinId;
                personelEntity.Modified = DateTime.Now;
                if (!string.IsNullOrEmpty(model.BirthDateStr))
                {
                    var dtBirthDate = _unitOfWork.BaseConfig.StringToDate(model.BirthDateStr);
                    personelEntity.BirthDate = dtBirthDate;
                }
                else
                {
                    personelEntity.BirthDate = null;
                }
                _unitOfWork.Personeli.Update(personelEntity);
            }

       
            await _unitOfWork.Personeli.Commit();

            var menuToReturn = _mapper.Map<PersonelDto>(personelEntity);
            return Ok(menuToReturn);

        }


        [Authorize]
        [HttpDelete("DeletePersonel")]
        public async Task<IActionResult> DeletePersonel(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var personelId = await _unitOfWork.Personeli.GetPersonelById(id, webLangId);

            if (personelId == null)
            {
                return NotFound();
            }

            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multiPersonelEntity = await _unitOfWork.Personeli.GetPersonelById(id, item.Id);
                    if (multiPersonelEntity != null)
                    {
                        _unitOfWork.Personeli.Delete(multiPersonelEntity);
                        await _unitOfWork.Personeli.Commit();
                    }
                }

                return Ok();
            }

            _unitOfWork.Personeli.Delete(personelId);

            await _unitOfWork.Personeli.Commit();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("OrderPersonel")]
        public async Task<ActionResult> OrderPersonel(List<UpdateOrderPersonelDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var personel = await _unitOfWork.Personeli.GetPersonelById(item.Id, item.LanguageId);
                    if (personel != null)
                    {
                        personel.OrderNo = item.OrderNo;
                        _unitOfWork.Personeli.Update(personel);
                        await _unitOfWork.Personeli.Commit();
                    }
                }
                return Ok();
            }

            return NotFound();
        }

        private async Task<bool> UpdatePersonelOrderBack(List<UpdateOrderPersonelDto>? model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var personel = await _unitOfWork.Personeli.GetPersonelById(item.Id, item.LanguageId);
                    if (personel != null)
                    {
                        personel.OrderNo = item.OrderNo + 1;
                        _unitOfWork.Personeli.Update(personel);
                    }
                }
                await _unitOfWork.Personeli.Commit();
                return true;
            }
            return false;
        }
    }
}
