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
    public class GaleryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public GaleryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetGaleries")]
        public async Task<IActionResult> GetGaleries(int MediaGaleriaKategoriaID, int PageID = 0, string Layout = "default", string Gjuha = "sq", int skip = 0, int take = 10)
        {
           
                int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

                if (PageID == 0)
                {
                    PageID = await _unitOfWork.GaleryHeaders.GetPageId(MediaGaleriaKategoriaID, Layout);
                }

                var page = await _unitOfWork.Pages.GetBasicPage(PageID, gjuhaID, _unitOfWork.BaseConfig.GetDateTime());

                var galeriaLista = await _unitOfWork.GaleryHeaders.GetGaleries(gjuhaID, MediaGaleriaKategoriaID, page.FirstOrDefault().LayoutId, page.FirstOrDefault().PageId, skip, take);

                var totalRowsCount = await _unitOfWork.GaleryHeaders.GetGaleriesCount(gjuhaID, MediaGaleriaKategoriaID, page.FirstOrDefault().LayoutId, page.FirstOrDefault().PageId);

                int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);

                if (page != null)
                    return Ok(new { page, galeriaLista, totalRowsCount, totalPages });
                else
                    return NotFound();
        
        }

        [HttpGet("GetGaleryDetails")]
        public async Task<IActionResult> GetGaleryDetails(int MediaGaleriaID, string Gjuha = "sq")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var galeriaLista = await _unitOfWork.GaleryDetails.GetGaleryDetails(MediaGaleriaID, gjuhaID);

            if (galeriaLista.Count > 0)
                return Ok(galeriaLista);
            else
                return NotFound();
        }
    }
}
