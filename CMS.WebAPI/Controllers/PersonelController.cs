using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;

namespace CMS.WebAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public PersonelController(IUnitOfWork unitOfWorkRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWorkRepository;
            _mapper = mapper;
        }

        [HttpGet("GetPersonels")]
        public async Task<IActionResult> GetPersonels(int PageID, string Gjuha = "sq", int skip = 0, int take = 10)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

            var result = await _unitOfWork.Personeli.GetPersonel(gjuhaID, PageID, skip, take);

            var totalRowsCount = await _unitOfWork.Personeli.GetPersonelCount(gjuhaID, PageID);

            int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);

            if (result.Count > 0)
                return Ok(new { result, totalRowsCount, totalPages });
            else
                return NotFound();
        }

        [HttpGet("GetPersonelDetails")]
        public async Task<IActionResult> GetPersonelDetails(int PersoneliID, string Gjuha = "sq")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Personeli.GetPersonelDetails(gjuhaID, PersoneliID);

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        //[HttpGet("GetPersonels")]
        //public async Task<IActionResult> GetPersonels(int PageID, string Gjuha = "sq", int skip = 0, int take = 10)
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

        //    var result = await _unitOfWork.Personeli.FindByCondition(t => t.LanguageId == gjuhaID && t.PageId == PageID, false, new[] { "Layout", "Media" }).OrderBy(t => t.OrderNo).Skip(skip).Take(take).ToListAsync();

        //    var personelMapped = _mapper.Map<List<Personel>, List<PersonelModel>>(result);

        //    var totalRowsCount = personelMapped.Count;

        //    int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);

        //    if (result.Count > 0)
        //        return Ok(new { result = personelMapped, totalRowsCount, totalPages });
        //    else
        //        return NotFound();
        //}

        //[HttpGet("GetPersonelDetails")]
        //public async Task<IActionResult> GetPersonelDetails(int PersoneliID, string Gjuha = "sq")
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var result = await _unitOfWork.Personeli.FindByCondition(t => t.LanguageId == gjuhaID && t.Id == PersoneliID, false, new[] { "Layout", "Media" }).FirstOrDefaultAsync();
        //    var personelMapped = _mapper.Map<Personel, PersonelModel>(result);

        //    return Ok(personelMapped);
        //}
    }
}
