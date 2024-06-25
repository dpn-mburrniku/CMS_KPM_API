using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTranslationController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public ResourceTranslationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [Route("GetResourceTranslationType")]
        public async Task<IActionResult> GetResourceTranslationType() { 

            var translationType = await _unitOfWork.ResourceTranslation.GetResourceTranslationType();
          
            return Ok(translationType);
        }


        [Authorize]
        [HttpGet]
        [Route("GetResourceTranslationById")]
        public async Task<IActionResult> GetResourceTranslationById(int id, int webLangId) { 

            var resource = await _unitOfWork.ResourceTranslation.GetResourceTranslationById(id, webLangId);

            if (resource == null)
            {
                return NotFound();
            }
            else {

                var resourceDto = _mapper.Map<ResourceTranslationStringDto>(resource);
                return Ok(resourceDto);
            }

        }

        [Authorize]
        [HttpGet]
        [Route("AllIdsExist")]
        public async Task<IActionResult> AllIdsExist(int webLangId) {

            var resources = await _unitOfWork.ResourceTranslation.GetResourceTranslationByLang(1);

            foreach (var resource in resources)
            {
                var id = resource.Id;
                    bool exist = _unitOfWork.ResourceTranslation.ResourceExist(webLangId, id);
                    if (!exist)
                    {
                       return Ok(false);
                }
            }
            return Ok(true); 
        }

        [Authorize]
        [HttpPost]
        [Route("ImportResourceTranslation")]

        public async Task<IActionResult> ImportResourceTranslationAsync(int webLangId)
        {
            var resources = await _unitOfWork.ResourceTranslation.GetResourceTranslationByLang(1);

            foreach (var resource in resources)
            {
                var id = resource.Id;
                bool exist = _unitOfWork.ResourceTranslation.ResourceExist(webLangId, id);

                if (!exist)
                {
                    var newResource = _mapper.Map<ResourceTranslationString>(resource);
                    newResource.LanguageId = webLangId;

                    await _unitOfWork.ResourceTranslation.Create(newResource);
                    await _unitOfWork.ResourceTranslation.Commit();
                }
            }
            return StatusCode(StatusCodes.Status201Created);

        }

        [Authorize]
        [HttpPost]
        [Route("GetResourceTranslationAsync")]
        public async Task<IActionResult> GetResourceTranslationAsync([FromBody] ResourceTranslationStringFilterParameters parameter)
        {

            var translations = await _unitOfWork.ResourceTranslation.GetResourceTranslationAsync(parameter);
            var translationsDTO = _mapper.Map<List<ResourceTranslationStringListDto>>(translations);

            return Ok(new
            {
                data = translationsDTO,
                total = translations.MetaData.TotalCount
            });
        }



        [Authorize]
        [HttpPost]
        [Route("CreateResourceTranslation")]
        public async Task<IActionResult> CreateResourceTranslationAsync([FromBody] ResourceTranslationStringDto model) {

            if (ModelState.IsValid)
            {
                int resourceTranslationId = _unitOfWork.ResourceTranslation.GetMaxPK(a => a.Id);
                model.Id = resourceTranslationId;
                var langList = await _unitOfWork.BaseConfig.GetLangList();

                foreach (var lang in langList)
                {
                    var resourceTranslationEntity = _mapper.Map<ResourceTranslationString>(model);
                    resourceTranslationEntity.LanguageId = lang.Id;

                    await _unitOfWork.ResourceTranslation.Create(resourceTranslationEntity);
                    await _unitOfWork.ResourceTranslation.Commit();
                }
                return StatusCode(StatusCodes.Status201Created);
            }else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateResourceTranslation")]
        public async Task<IActionResult> UpdateResourceTranslationAsync([FromBody] ResourceTranslationStringDto model) {
            var resourceTranslationEntity = await _unitOfWork.ResourceTranslation.GetResourceTranslationById(model.Id, model.LanguageId);
            if (resourceTranslationEntity == null) {
                return NotFound();
            }
            _mapper.Map(model, resourceTranslationEntity);
            _unitOfWork.ResourceTranslation.Update(resourceTranslationEntity);
            await _unitOfWork.ResourceTranslation.Commit();

            var updatedResource = _mapper.Map<ResourceTranslationStringDto>(resourceTranslationEntity);
            return Ok(updatedResource);
        }

        [Authorize]
        [HttpDelete("DeleteResourceTranslation")]
        public async Task<IActionResult> DeleteResourceTranslationAsync(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var resource = await _unitOfWork.ResourceTranslation.GetResourceTranslationById(id, webLangId);

            if (resource == null)
            {
                return NotFound();
            }

            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var lang in langList)
                {
                    var resourceTranslation = await _unitOfWork.ResourceTranslation.GetResourceTranslationById(id, lang.Id);
                    if (resourceTranslation != null)
                    {
                        _unitOfWork.ResourceTranslation.Delete(resourceTranslation);
                        await _unitOfWork.ResourceTranslation.Commit();
                    }

                }
            }
            else
            {
                _unitOfWork.ResourceTranslation.Delete(resource);

                await _unitOfWork.Links.Commit();
            }
            return Ok();
        }
    }
}

