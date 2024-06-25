using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;
using Entities.DataTransferObjects;

namespace CMS.WebAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class MunicipaltyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public MunicipaltyController(IUnitOfWork unitOfWork
                    , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetMunicipalities")]
        public async Task<IActionResult> GetMunicipalities()
        {
            var municipalities = await _unitOfWork.Municipality.FindByCondition(t => t.Active == true, false, null).ToListAsync();

            var municipalitiesDto = _mapper.Map<IEnumerable<MunicipalityDto>?>(municipalities);

            return Ok(municipalitiesDto);
        }

        [HttpGet]
        [Route("GetMunicipalityLocation")]
        public async Task<IActionResult> GetMunicipalityLocation(int? municiplaityId)
        {
            string[] includes = { "Municipality", "MeasureUnit" };
            var GetMunicipalityLocationList = await _unitOfWork.MunicipalityLocations.FindByCondition(t => t.Active == true && t.MunicipalityId == municiplaityId, false, includes).ToListAsync();
            var MunicipalityLocationDto = _mapper.Map<IEnumerable<MunicipalityLocationListDto>?>(GetMunicipalityLocationList);
            return Ok(MunicipalityLocationDto);

        }


    }
}
