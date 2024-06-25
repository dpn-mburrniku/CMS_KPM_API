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
    public class SideBarController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public SideBarController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetMenus")]
        public async Task<IActionResult> GetMenus(string Gjuha = "sq", string LocationMenuID = "2", int ParentMenuID = 0, int ParentPageID = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

            List<int> _LocationMenuIDs = LocationMenuID?.Split(',')?.Select(Int32.Parse)?.ToList();

            List<MenuModel> result = new List<MenuModel>();
            foreach (var item in _LocationMenuIDs)
            {
                var result1 = await _unitOfWork.Menu.GetSideBarMenus(gjuhaID, item, ParentMenuID, ParentPageID);

                if (result1.Count > 0)
                {
                    result.AddRange(result1);
                }
                else
                {

                    var menu = await _unitOfWork.Menu.GetMenuByPageID(ParentPageID, gjuhaID, item);
                    if (menu.Count > 0 && menu.FirstOrDefault().PagePrindId != null)
                    {
                        var result2 = await _unitOfWork.Menu.GetSideBarMenus(gjuhaID, item, ParentMenuID, (int)menu.FirstOrDefault().PagePrindId);
                        if (result2.Count > 0)
                        {
                            result.AddRange(result2);
                        }
                    }
                    else
                    {
                        var result3 = await _unitOfWork.Menu.GetSideBarMenus(gjuhaID, item, ParentMenuID, 0);
                        if (result3.Count > 0)
                        {
                            result.AddRange(result3);
                        }
                    }
                }
            }

            return Ok(result);
        }

        [HttpGet("GetLinks")]
        public async Task<IActionResult> GetLinks(string Gjuha = "sq", int LinkTypeID = 2, int PageID = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Links.GetLinks(gjuhaID, LinkTypeID, PageID);

            return Ok(result);
        }

        //[HttpGet("GetMenus")]
        //public async Task<IActionResult> GetMenus(int? ParentMenuID, int? ParentPageID, string Gjuha = "sq", string MenuTypeId = "2")
        //{
        //    string[] includes = { 
        //                          "InverseMenuNavigation",
        //                          "InverseMenuNavigation.Page",
        //                          "InverseMenuNavigation.Page.Template",
        //                          "InverseMenuNavigation.Page.Layout",
        //                          "InverseMenuNavigation.PageNavigation",
        //                          "InverseMenuNavigation.PageNavigation.Template",
        //                          "InverseMenuNavigation.PageNavigation.Layout",

        //                          "InverseMenuNavigation.InverseMenuNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.Page",
        //                          "InverseMenuNavigation.InverseMenuNavigation.Page.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.Page.Layout",
        //                          "InverseMenuNavigation.InverseMenuNavigation.PageNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Layout",

        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Layout",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Layout",

        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Template",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Layout",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Template",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Layout",

        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Template",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Layout",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Template",
        //                          //"InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Layout",

        //                          "MenuNavigation",
        //                          "MenuType",
        //                          "Page",
        //                          "Page.Template",
        //                          "Page.Layout",
        //                          "PageNavigation",
        //                          "PageNavigation.Template",
        //                          "PageNavigation.Layout" };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

        //    List<int> _LocationMenuIDs = MenuTypeId?.Split(',')?.Select(Int32.Parse)?.ToList();

        //    List<MenuModel> result = new List<MenuModel>();
        //    foreach (var item in _LocationMenuIDs)
        //    {
        //        var result1 = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == item && x.MenuParentId == ParentMenuID &&
        //                                                                   (ParentPageID != null ? x.PageParentId == ParentPageID : x.PageParentId == null),
        //                                                                    false, includes).OrderBy(t => t.OrderNo).ToListAsync();

        //        var listMenu1 = _mapper.Map<List<Menu>, List<MenuModel>>(result1);

        //        if (listMenu1.Count > 0)
        //        {
        //            result.AddRange(listMenu1);
        //        }
        //        else
        //        {

        //            var menu = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == item && x.PageId == ParentPageID,
        //                                                                    false, includes).ToListAsync();
        //            if (menu.Count > 0 && menu.FirstOrDefault().PageParentId != null)
        //            {
        //                var result2 = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == item && x.MenuParentId == ParentMenuID &&
        //                                                                    ((int)menu.FirstOrDefault().PageParentId > 0 ? x.PageParentId == (int)menu.FirstOrDefault().PageParentId : x.PageParentId == null || x.PageParentId == 0),
        //                                                                    false, includes).OrderBy(t => t.OrderNo).ToListAsync();

        //                var listMenu2 = _mapper.Map<List<Menu>, List<MenuModel>>(result1);
        //                if (listMenu2.Count > 0)
        //                {
        //                    result.AddRange(listMenu2);
        //                }
        //            }
        //            else
        //            {
        //                var result3 = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == item && x.MenuParentId == ParentMenuID &&
        //                                                                    (x.PageParentId == 0),
        //                                                                    false, includes).OrderBy(t => t.OrderNo).ToListAsync();

        //                var listMenu3 = _mapper.Map<List<Menu>, List<MenuModel>>(result1);
        //                if (listMenu3.Count > 0)
        //                {
        //                    result.AddRange(listMenu3);
        //                }
        //            }
        //        }
        //    }

        //    //if (result.Count > 0)
        //        return Ok(result);
        //    //else
        //    //    return NotFound();
        //}

        //[HttpGet("GetLinks")]
        //public async Task<IActionResult> GetLinks(string Gjuha = "sq", int LinkTypeID = 2, int PageID = 0)
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

        //    var linksLista = new List<LinksModel>();

        //    var page = await _unitOfWork.Pages.GetPageById(PageID, gjuhaID);

        //    if (page != null)
        //    {
        //        var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.PageId == PageID, false, new[] { "LinkType" }).OrderBy(t => t.OrderNo).ToListAsync();
        //        var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //        linksLista.AddRange(Lista);
        //    }
        //    if (linksLista.Count == 0 && page != null)
        //    {
        //        var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.LayoutId == page.LayoutId, false, new[] { "LinkType" }).OrderBy(t => t.OrderNo).ToListAsync();
        //        var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //        linksLista.AddRange(Lista);
        //    }
        //    //if (linksLista.Count == 0)
        //    //{
        //    //    var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.PageId == null, false, new[] { "LinkType" }).ToListAsync();
        //    //    var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //    //    linksLista.AddRange(Lista);
        //    //}
        //    return Ok(linksLista);
        //}
    }
}
