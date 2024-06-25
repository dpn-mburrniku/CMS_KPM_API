using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities.RequestFeatures;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public LanguageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [Route("GetLanguages")]
        public async Task<IActionResult> GetLanguages()
        {

            var languages = await _unitOfWork.Language.FindByCondition(t => t.Active == true, false, null).ToListAsync();

            var languagesDto = _mapper.Map<IEnumerable<LanguageListDto>?>(languages);

            return Ok(languagesDto);
        }

        [Authorize]
        [HttpGet]
        [Route("GetCultureCodes")]
        public async Task<IActionResult> GetCultureCodes()
        {
            var cultureCodes = await _unitOfWork.Language.GetCultureCode();

            return Ok(cultureCodes);
        }

        [Authorize]
        [HttpPost]
        [Route("GetLanguagesAsync")]
        public async Task<IActionResult> GetLanguagesAsync([FromBody] LanguageParameters parameter)
        {
            var language = await _unitOfWork.Language.GetLanguageAsync(parameter);

            var languageDto = _mapper.Map<IEnumerable<LanguageListDto>>(language);

            return Ok(new
            {
                data = languageDto,
                total = language.MetaData.TotalCount
            });

        }

        [Authorize]
        [HttpGet("GetLanguageById")]
        public async Task<IActionResult> GetLanguageById(int id)
        {
            var data = await _unitOfWork.Language.GetLanguageById(id);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<LanguageDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateLanguage")]
        public async Task<IActionResult> CreateLanguage([FromBody] LanguageDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int languageId = _unitOfWork.Language.GetMaxPK(a => a.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = languageId;

                var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var languageEntity = _mapper.Map<Language>(model);

                await _unitOfWork.Language.Create(languageEntity);
                await _unitOfWork.Language.Commit();

                return StatusCode(StatusCodes.Status201Created, languageEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut("UpdateLanguage")]
        public async Task<IActionResult> UpdateLanguageAsync([FromBody] UpdateLanguageDto language)
        {
            var languageEntity = await _unitOfWork.Language.GetById(language.Id);

            if (languageEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(language, languageEntity);
            _unitOfWork.Language.Update(languageEntity);
            await _unitOfWork.Language.Commit();

            var languageToReturn = _mapper.Map<LanguageDto>(languageEntity);

            return Ok(languageToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteLanguage")]
        public async Task<IActionResult> DeleteLanguageAsync(int id)
        {
            try
            {
                var language = await _unitOfWork.Language.GetById(id);

                if (language == null)
                {
                    return NotFound();
                }

                _unitOfWork.Language.Delete(language);

                await _unitOfWork.Language.Commit();

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "This language is used in another table");
            }
        }


    }
}
