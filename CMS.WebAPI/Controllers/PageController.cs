using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;
using NetTopologySuite.Index.HPRtree;

namespace CMS.WebAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public PageController(IUnitOfWork unitOfWorkRepository
                    , IMapper mapper)
        {
            _unitOfWork = unitOfWorkRepository;
            _mapper = mapper;
        }

        [HttpGet("GetBasicPage")]
        public async Task<IActionResult> GetBasicPage(int PageID, string Gjuha = "sq")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();

            var pageLista = await _unitOfWork.Pages.GetBasicPage(PageID, gjuhaID, data);

            if (pageLista.Count > 0)
                return Ok(pageLista);
            else
                return NotFound();
        }

        [HttpGet("GetPageWithDocs")]
        public async Task<IActionResult> GetPageWithDocs(int PageID, string Gjuha = "sq", int skip = 0, int take = 10)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();

            var page = await _unitOfWork.Pages.GetBasicPage(PageID, gjuhaID, data);

            var mediaLista = await _unitOfWork.Pages.GetPageMedia(PageID, gjuhaID, skip, take, 0, "", data);

            int totalmediaRowsCount = await _unitOfWork.Pages.GetPageMediaCount(PageID, gjuhaID, 0, "", data);

            var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

            if (page.Count > 0)
                return Ok(new { page, mediaLista, totalmediaRowsCount, totalmediaPages });
            else
                return NotFound();
        }

        [HttpGet("GetPageMedia")]
        public async Task<IActionResult> GetPageMedia(int PageID, string Gjuha = "sq", int skip = 0, int take = 10, string? searchText = "")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();

            var mediaLista = await _unitOfWork.Pages.GetPageMedia(PageID, gjuhaID, skip, take, 0, searchText, data);

            int totalmediaRowsCount = await _unitOfWork.Pages.GetPageMediaCount(PageID, gjuhaID, 0, searchText, data);

            var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

            if (mediaLista != null)
                return Ok(new { mediaLista, totalmediaRowsCount, totalmediaPages });
            else
                return NotFound();
        }

        [HttpGet("GetPageWithDocsAndFilter")]
        public async Task<IActionResult> GetPageWithDocsAndFilter(int PageID, string Gjuha = "sq", int skip = 0, int take = 10, int Viti = 0, string? searchText = "")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();

            var page = await _unitOfWork.Pages.GetBasicPage(PageID, gjuhaID, data);

            var mediaLista = await _unitOfWork.Pages.GetPageMedia(PageID, gjuhaID, skip, take, Viti, searchText, data);

            var slider = await _unitOfWork.Pages.GetPageSlider(PageID, gjuhaID);

            int totalmediaRowsCount = await _unitOfWork.Pages.GetPageMediaCount(PageID, gjuhaID, Viti, searchText, data);

            var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

            var vitet = await _unitOfWork.Pages.GetVitet(PageID, gjuhaID, data);

            if (page.Count > 0)
                return Ok(new { page, mediaLista, slider, vitet, totalmediaRowsCount, totalmediaPages });
            else
                return NotFound();
        }

        [HttpGet("GetPageSubPages")]
        public async Task<IActionResult> GetPageSubPages(int ParentPageID, string Gjuha = "sq", int skip = 0, int take = 10, int Viti = 0, string? searchText = "", int? top = null)
        {
            List<SubPagesListModel> returnList = new List<SubPagesListModel>();
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();

            var subPages = await _unitOfWork.Pages.GetSubPages(ParentPageID, gjuhaID, data, top);
            int id = 1;

            foreach (var item in subPages.ToList().OrderByDescending(t => t.PageId))
            {

                var page = await _unitOfWork.Pages.GetBasicPage(item.PageId, gjuhaID, data);

                var mediaLista = await _unitOfWork.Pages.GetPageMedia(item.PageId, gjuhaID, skip, take, Viti, searchText, data);

                var slider = await _unitOfWork.Pages.GetPageSlider(item.PageId, gjuhaID);

                int totalmediaRowsCount = await _unitOfWork.Pages.GetPageMediaCount(item.PageId, gjuhaID, Viti, searchText, data);

                var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

                var vitet = await _unitOfWork.Pages.GetVitet(item.PageId, gjuhaID, data);


                var lista = new SubPagesListModel();
                lista.Id = id++;
                lista.page = page.FirstOrDefault();
                lista.media = mediaLista;
                lista.slider = slider;
                lista.totalmediaRowsCount = totalmediaRowsCount;
                lista.totalmediaPages = totalmediaPages;
                lista.vitet = vitet;
                returnList.Add(lista);
            }

            if (returnList.Count > 0)
                return Ok(returnList);
            else
                return NotFound();
        }
        //[HttpGet("GetBasicPage")]
        //public async Task<IActionResult> GetBasicPage(int PageID, string Gjuha = "sq")
        //{
        //    string[] includes = { "Template", "Layout", "Media", "Media.FileExNavigation" };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

        //    var page = await _unitOfWork.Pages.FindByCondition(x=>x.Id == PageID && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

        //    var mappedPage = _mapper.Map<Page, BasicPageModel>(page);

        //    return Ok(mappedPage);
        //}

        //[HttpGet("GetPageWithDocs")]
        //public async Task<IActionResult> GetPageWithDocs(int PageID, string Gjuha = "sq", int skip = 0, int take = 10)
        //{
        //    string[] includes = { "InversePageNavigation",
        //                          "Template",
        //                          "Layout",
        //                          "Media",
        //                          "Media.FileExNavigation" };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

        //    var page = await _unitOfWork.Pages.FindByCondition(x => x.Id == PageID && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

        //    var mappedPage = _mapper.Map<Page, PageModel>(page);

        //    var mediaLista = mappedPage.PageMedia.Where(pm => pm.IsSlider == false 
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)).OrderBy(t => t.OrderNo).Skip(skip).Take(take).ToList();

        //    int totalmediaRowsCount = mappedPage.PageMedia.Count;

        //    var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

        //    if (page != null)
        //        return Ok(new { page = mappedPage, mediaLista, totalmediaRowsCount, totalmediaPages });
        //    else
        //        return NotFound();
        //}

        //[HttpGet("GetPageMedia")]
        //public async Task<IActionResult> GetPageMedia(int PageID, string Gjuha = "sq", int skip = 0, int take = 10, string? searchText = "")
        //{
        //    string[] includes = { "Media",
        //                          "Media.FileExNavigation", };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

        //    var pageMediaLista = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == false
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), true, includes).OrderBy(t => t.OrderNo).Skip(skip).Take(take).ToListAsync();

        //    var mappedPageMedia = _mapper.Map<List<PageMedium>, List<PageMediaModel>>(pageMediaLista);

        //    int totalmediaRowsCount = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == false
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), true, includes).CountAsync(); 

        //    var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

        //    if (mappedPageMedia != null)
        //        return Ok(new { mediaLista = mappedPageMedia, totalmediaRowsCount, totalmediaPages });
        //    else
        //        return NotFound();
        //}

        //[HttpGet("GetPageWithDocsAndFilter")]
        //public async Task<IActionResult> GetPageWithDocsAndFilter(int PageID, string Gjuha = "sq", int skip = 0, int take = 10, int Viti = 0, string? searchText = "")
        //{
        //    string[] includes = { "Template",
        //                          "Layout",
        //                          "Media",
        //                          "Media.FileExNavigation" };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

        //    var page = await _unitOfWork.Pages.FindByCondition(x => x.Id == PageID && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

        //    //var mappedBasicPage = _mapper.Map<Page, BasicPageModel>(page);

        //    var mappedPage = _mapper.Map<Page, BasicPageModel>(page);

        //    string[] pageMediaIncludes = { "Media", "Media.FileExNavigation", };
        //    var slider = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == true
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, pageMediaIncludes).OrderBy(t => t.OrderNo).ToListAsync();

        //    var mappedslider = _mapper.Map<List<PageMedium>, List<PageMediaModel>>(slider);

        //    var pageMediaLista = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == false
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, pageMediaIncludes).OrderBy(t => t.OrderNo).Skip(skip).Take(take).ToListAsync();

        //    var mappedPageMedia = _mapper.Map<List<PageMedium>, List<PageMediaModel>>(pageMediaLista);

        //    int totalmediaRowsCount = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == false
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, null).CountAsync();

        //    var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

        //    var vitet = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == PageID && pm.LanguageId == gjuhaID
        //                                            && pm.IsSlider == false && pm.StartDate != null
        //                                            && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                            && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                            && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, null).OrderBy(t => t.StartDate).Select(t=>t.StartDate.Value.Year).Distinct().Cast<int>().ToListAsync();

        //    mappedPage.hasMedia = totalmediaRowsCount > 0;
        //    mappedPage.hasSlider = mappedslider.Count > 0;
        //    if (page != null)
        //        return Ok(new { page = mappedPage, mediaLista = mappedPageMedia, slider = mappedslider, vitet, totalmediaRowsCount, totalmediaPages });
        //    else
        //        return NotFound();
        //}

        //[HttpGet("GetPageSubPages")]
        //public async Task<IActionResult> GetPageSubPages(int PageID, string Gjuha = "sq", int skip = 0, int take = 10, int Viti = 0, string? searchText = "", int top = 0)
        //{
        //    List<SubPagesListModel> returnList = new List<SubPagesListModel>();
        //    string[] includes = { "InversePageNavigation",
        //                          "Template",
        //                          "Layout",
        //                          "Media",
        //                          "Media.FileExNavigation"};

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

        //    var page = await _unitOfWork.Pages.FindByCondition(x => x.Id == PageID && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

        //    var mappedPage = _mapper.Map<Page, PageModel>(page);
        //    int id = 1;

        //    foreach (var item in top > 0 ? mappedPage.subPages.OrderByDescending(t => t.Id).Take(top) : mappedPage.subPages.OrderByDescending(t => t.Id))
        //    {

        //        string[] pageMediaIncludes = { "Media", "Media.FileExNavigation", };
        //        var slider = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == item.Id && pm.LanguageId == gjuhaID
        //                                                && pm.IsSlider == true
        //                                                && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                                && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                                && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, pageMediaIncludes).OrderBy(t => t.OrderNo).ToListAsync();

        //        var mappedslider = _mapper.Map<List<PageMedium>, List<PageMediaModel>>(slider);

        //        var pageMediaLista = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == item.Id && pm.LanguageId == gjuhaID
        //                                                && pm.IsSlider == false
        //                                                && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                                && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                                && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, pageMediaIncludes).OrderBy(t => t.OrderNo).Skip(skip).Take(take).ToListAsync();

        //        var mappedPageMedia = _mapper.Map<List<PageMedium>, List<PageMediaModel>>(pageMediaLista);

        //        int totalmediaRowsCount = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == item.Id && pm.LanguageId == gjuhaID
        //                                                && pm.IsSlider == false
        //                                                && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                                && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                                && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, null).CountAsync();

        //        var totalmediaPages = _unitOfWork.BaseConfig.GetTotalPages(totalmediaRowsCount, take);

        //        var vitet = await _unitOfWork.PageMedia.FindByCondition(pm => pm.PageId == item.Id && pm.LanguageId == gjuhaID
        //                                                && pm.IsSlider == false && pm.StartDate != null
        //                                                && (pm.StartDate != null ? pm.StartDate.Value <= data : true)
        //                                                && (pm.EndDate != null ? pm.EndDate.Value >= data : true)
        //                                                && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true), false, null).OrderBy(t => t.StartDate).Select(t => t.StartDate.Value.Year).Distinct().Cast<int>().ToListAsync();

        //        item.hasMedia = totalmediaRowsCount > 0;
        //        item.hasSlider = mappedslider.Count > 0;


        //        var lista = new SubPagesListModel();
        //        lista.Id = id++;
        //        lista.page = item;
        //        lista.media = mappedPageMedia;
        //        lista.slider = slider;
        //        lista.totalmediaRowsCount = totalmediaRowsCount;
        //        lista.totalmediaPages = totalmediaPages;
        //        lista.vitet = vitet;


        //        returnList.Add(lista);
        //    }

        //    if (returnList.Count > 0)
        //        return Ok(returnList);
        //    else
        //        return NotFound();
        //}
    }
}
