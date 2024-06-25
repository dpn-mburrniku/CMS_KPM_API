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
    public class MunicipalityController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public MunicipalityController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [Route("GetMunicipalities")]
        public async Task<IActionResult> GetMunicipalities()
        {

            var municipalities = await _unitOfWork.Municipality.FindByCondition(t => t.Active == true, false, null).OrderBy(t => t.NameSq).ToListAsync();

            var municipalitiesDto = _mapper.Map<IEnumerable<MunicipalityDto>?>(municipalities);

            return Ok(municipalitiesDto);
        }

        [Authorize]
        [HttpPost]
        [Route("GetMunicipalitiesAsync")]
        public async Task<IActionResult> GetMunicipalitiesAsync([FromBody] FilterParameters parameter)
        {
            var municipality = await _unitOfWork.Municipality.GetMunicipalitiesAsync(parameter);

            var municipalityDto = _mapper.Map<IEnumerable<MunicipalityDto>>(municipality);

            return Ok(new
            {
                data = municipalityDto,
                total = municipality.MetaData.TotalCount
            });

        }

        [Authorize]
        [HttpGet("GetMunicipalityById")]
        public async Task<IActionResult> GetMunicipalityById(int id)
        {
            var data = await _unitOfWork.Municipality.GetMunicipalityById(id);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<MunicipalityDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateMunicipalities")]

        public async Task<IActionResult> CreateMunicipalities([FromBody] MunicipalityDto model)
        {
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var municipalityEntity = _mapper.Map<Municipality>(model);
            municipalityEntity.Created = DateTime.Now;
            municipalityEntity.CreatedBy = userId;

            await _unitOfWork.Municipality.Create(municipalityEntity);
            await _unitOfWork.Municipality.Commit();

            var layoutToReturn = _mapper.Map<MunicipalityDto>(municipalityEntity);

            return CreatedAtRoute("LayoutById", new { id = layoutToReturn.Id },
                                   layoutToReturn);
        }

        [Authorize]
        [HttpPut("UpdateMunicipality")]
        public async Task<IActionResult> UpdateMunicipalityAsync([FromBody] UpdateMunicipalityDto municipality)
        {
            var municipalityEntity = await _unitOfWork.Municipality.GetById(municipality.Id);

            if (municipalityEntity == null)
            {
                return NotFound();
            }
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            _mapper.Map(municipality, municipalityEntity);
            municipalityEntity.Modified = DateTime.Now;
            municipalityEntity.ModifiedBy = userId;
            _unitOfWork.Municipality.Update(municipalityEntity);
            await _unitOfWork.Municipality.Commit();

            var municipalitiesToReturn = _mapper.Map<MunicipalityDto>(municipalityEntity);

            return Ok(municipalitiesToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteMunicipality")]
        public async Task<IActionResult> DeleteMunicipalityAsync(int id)
        {
            try
            {
                var municipality = await _unitOfWork.Municipality.GetById(id);

                if (municipality == null)
                {
                    return NotFound();
                }

                _unitOfWork.Municipality.Delete(municipality);

                await _unitOfWork.Municipality.Commit();

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "This municipality is used in another table");
            }
        }


    }
}
