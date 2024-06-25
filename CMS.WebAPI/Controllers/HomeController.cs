using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;
using Entities.DataTransferObjects;
using CMS.WebAPI.ExternalServices;

namespace CMS.WebAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        private readonly IAirQualityServices _airQualityServices;
        public HomeController(IUnitOfWork unitOfWork
                    , IMapper mapper
                    , IAirQualityServices airQualityServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _airQualityServices = airQualityServices;
        }

        //[HttpGet("GetMenus")]
        //public async Task<IActionResult> GetMenus(int? ParentMenuID, string Gjuha = "sq", int MenuTypeID = 1)
        //{
        //    string[] includes = { "MenuNavigation",
        //                          "MenuType",
        //                          "Page",
        //                          "Page.Media",
        //                          "Page.Template",
        //                          "Page.Layout",
        //                          "PageNavigation",
        //                          "PageNavigation.Template",
        //                          "PageNavigation.Layout",

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
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page.Layout",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Template",
        //                          "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.PageNavigation.Layout"
        //                        };
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var result = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID && x.MenuParentId == ParentMenuID, 
        //                                        false, includes).OrderBy(t=>t.OrderNo).ToListAsync();

        //    var listMenu = _mapper.Map<List<Menu>, List<MenuModel>>(result);
        //    return Ok(listMenu);

        //}

        [HttpGet("GetMenus")]
        public async Task<IActionResult> GetMenus(string Gjuha = "sq", int LocationMenuID = 1, int ParentMenuID = 0)
        { 
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Menu.GetMenus(gjuhaID, LocationMenuID, ParentMenuID);
            return Ok(result);
        }


        //[HttpGet("GetMenusWithoutChilds")]
        //public async Task<IActionResult> GetMenusWithoutChilds(int? ParentMenuID, string Gjuha = "sq", int MenuTypeID = 3)
        //{
        //    string[] includes = { "MenuNavigation",
        //                          "MenuType",
        //                          "Page",
        //                          "Page.Media",
        //                          "Page.Template",
        //                          "Page.Layout",
        //                          "PageNavigation",
        //                          "PageNavigation.Template",
        //                          "PageNavigation.Layout" };

        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var result = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID && x.MenuParentId == ParentMenuID,
        //                                        true, includes).OrderBy(t => t.OrderNo).ToListAsync();
        //    var listMenu = _mapper.Map<List<Menu>, List<MenuWithoutChildsModel>>(result);
        //    return Ok(listMenu);
        //}

        [HttpGet("GetMenusWithoutChilds")]
        public async Task<IActionResult> GetMenusWithoutChilds(string Gjuha = "sq", int LocationMenuID = 3, int ParentMenuID = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Menu.GetMenusWithoutChilds(gjuhaID, LocationMenuID, ParentMenuID);
            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        //[HttpGet("GetMenusAtoZ")]
        //public async Task<IActionResult> GetMenusAtoZ(string Gjuha = "sq", string? Letter = "")
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    if (string.IsNullOrEmpty(Letter))
        //    {
        //        var result = await GetMenusAtoZ(gjuhaID);
        //        if (result != null)
        //            return Ok(result);
        //        else
        //            return NotFound();
        //    }
        //    else
        //    {
        //        var result = await GetMenusAtoZWithLetter(gjuhaID, Letter);
        //        if (result != null)
        //            return Ok(result);
        //        else
        //            return NotFound();
        //    }
        //}
        //private async Task<List<MenuAtoZModel>> GetMenusAtoZ(int gjuhaID)
        //{
        //    var menuList = new List<MenuAtoZModel>();

        //    var getAllMenus = await getMenusStartWithLetter(gjuhaID);

        //    for (char c = 'A'; c < 'Z'; c++)
        //    {
        //        var menu = new MenuAtoZModel();
        //        menu.Letter = c.ToString();
        //        menu.menus = getAllMenus.Where(t => t.Page != null ? t.Page.PageName.StartsWith(c.ToString()) : t.OtherSourceName.StartsWith(c.ToString())).ToList();
        //        menuList.Add(menu);
        //    }
        //    return menuList;
        //}

        //private async Task<List<MenuAtoZModel>> GetMenusAtoZWithLetter(int gjuhaID, string Letter)
        //{
        //    var menuList = new List<MenuAtoZModel>();

        //    var getAllMenus = await getMenusStartWithLetter(gjuhaID);

        //    var menu = new MenuAtoZModel();
        //    menu.Letter = Letter;
        //    menu.menus = getAllMenus.Where(t => t.Page != null ? t.Page.PageName.StartsWith(Letter) : t.OtherSourceName.StartsWith(Letter)).ToList();

        //    menuList.Add(menu);

        //    return menuList;
        //}

        [HttpGet("GetMenusAtoZ")]
        public async Task<IActionResult> GetMenusAtoZ(string Gjuha = "sq", string? Letter = "")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            if (string.IsNullOrEmpty(Letter))
            {
                var result = await _unitOfWork.Menu.GetMenusAtoZ(gjuhaID);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound();
            }
            else
            {
                var result = await _unitOfWork.Menu.GetMenusAtoZWithLetter(gjuhaID, Letter);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound();
            }
        }

        //private async Task<List<MenuWithoutChildsModel>> getMenusStartWithLetter(int gjuhaID)
        //{
        //    int[] doNotShowmenus = { 163, 164, 165 };
        //    string[] includes = { "MenuNavigation",
        //                          "MenuType",
        //                          "Page",
        //                          "Page.Media",
        //                          "Page.Template",
        //                          "Page.Layout",
        //                          "PageNavigation",
        //                          "PageNavigation.Template",
        //                          "PageNavigation.Layout" };

        //    var result = await _unitOfWork.Menu.FindByCondition(x => x.Active != false && x.LanguageId == gjuhaID && !doNotShowmenus.Contains(x.PageId.Value),
        //                                        true, includes).OrderBy(t => t.OrderNo).ToListAsync();
        //    var listMenu = _mapper.Map<List<Menu>, List<MenuWithoutChildsModel>>(result);

        //    return listMenu;
        //}


        //[HttpGet("GetMenuPath")]
        //public async Task<IActionResult> GetMenuPath(int PageID, string Gjuha = "sq", int MenuTypeID = 1)
        //{
        //    List<MenuWithoutChildsModel> result = new List<MenuWithoutChildsModel>();
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    int niveli = 0;
        //    string[] includes = { "MenuNavigation",
        //                          "MenuType",
        //                          "Page",
        //                          "Page.Media",
        //                          "Page.Template",
        //                          "Page.Layout",
        //                          "PageNavigation",
        //                          "PageNavigation.Template",
        //                          "PageNavigation.Layout" };

        //    var menu = await _unitOfWork.Menu.FindByCondition(x => x.PageId == PageID && x.Active != false && x.LanguageId == gjuhaID,
        //                                        true, includes).FirstOrDefaultAsync();
        //    if (menu != null)
        //    {
        //        if (menu.Page != null)
        //        {
        //            if (menu.Page.LayoutId > 1)
        //            {
        //                var menulayout = await _unitOfWork.Menu.FindByCondition(x => x.Page.LayoutId == menu.Page.LayoutId && x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == 3,
        //                                            true, includes).FirstOrDefaultAsync();

        //                var listMenu = _mapper.Map<Menu, MenuWithoutChildsModel>(menulayout);
        //                if (listMenu != null)
        //                {
        //                    listMenu.Level = 0;
        //                    result.Add(listMenu);
        //                }
        //            }
        //        }
        //    }

        //    var listMenu1 = _mapper.Map<Menu, MenuWithoutChildsModel>(menu);
        //    if (listMenu1 != null) {
        //        result.Add(listMenu1);

        //        if (listMenu1.MenuParentId != null)
        //        {
        //            var menu2 = await _unitOfWork.Menu.FindByCondition(x => x.Id == listMenu1.MenuParentId && x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID,
        //                                        true, includes).FirstOrDefaultAsync();
        //            var listMenu2 = _mapper.Map<Menu, MenuWithoutChildsModel>(menu2);
        //            if (listMenu2 != null)
        //            {
        //                result.Add(listMenu2);

        //                if (listMenu2.MenuParentId != null)
        //                {
        //                    var menu3 = await _unitOfWork.Menu.FindByCondition(x => x.Id == listMenu2.MenuParentId && x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID,
        //                                                true, includes).FirstOrDefaultAsync();
        //                    var listMenu3 = _mapper.Map<Menu, MenuWithoutChildsModel>(menu3);
        //                    if (listMenu3 != null)
        //                    {
        //                        result.Add(listMenu3);

        //                        if (listMenu3.MenuParentId != null)
        //                        {
        //                            var menu4 = await _unitOfWork.Menu.FindByCondition(x => x.Id == listMenu3.MenuParentId && x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID,
        //                                                        true, includes).FirstOrDefaultAsync();
        //                            var listMenu4 = _mapper.Map<Menu, MenuWithoutChildsModel>(menu4);
        //                            if (listMenu4 != null)
        //                            {
        //                                result.Add(listMenu4);

        //                                if (listMenu4.MenuParentId != null)
        //                                {
        //                                    var menu5 = await _unitOfWork.Menu.FindByCondition(x => x.Id == listMenu4.MenuParentId && x.Active != false && x.LanguageId == gjuhaID && x.MenuTypeId == MenuTypeID,
        //                                                                true, includes).FirstOrDefaultAsync();
        //                                    var listMenu5 = _mapper.Map<Menu, MenuWithoutChildsModel>(menu5);
        //                                    if (listMenu5 != null)
        //                                    {
        //                                        result.Add(listMenu5);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (result.Count > 0)
        //        return Ok(result.DistinctBy(t => t.PageId).OrderBy(t => t.Level));
        //    else
        //        return NotFound();

        //}


        [HttpGet("GetMenuPath")]
        public async Task<IActionResult> GetMenuPath(int PageID, string Gjuha = "sq", int LocationMenuID = 1)
        {
            List<MenuModel> result = new List<MenuModel>();
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            int niveli = 0;
            var menu = await _unitOfWork.Menu.GetMenuByPageID(PageID, gjuhaID);
            if (menu.Count > 0)
            {
                if (menu.FirstOrDefault().LayoutID > 1)
                {
                    var result0 = await _unitOfWork.Menu.GetMenuPathLayoutHome((int)menu.FirstOrDefault().LayoutID, gjuhaID, 3, niveli);
                    if (result0.Count > 0)
                    {
                        result.AddRange(result0);
                        niveli = 1;
                    }
                }
            }
            if (menu.Count > 0 && menu.FirstOrDefault().PagePrindId != null)
            {
                var result1 = await _unitOfWork.Menu.GetMenuPath((int)menu.FirstOrDefault().PagePrindId, gjuhaID, LocationMenuID, niveli);
                if (result1.Count > 0)
                {
                    result.AddRange(result1);
                    niveli = (int)result1.Max(x => x.Niveli);
                }
            }
            var result2 = await _unitOfWork.Menu.GetMenuPath(PageID, gjuhaID, LocationMenuID, niveli);
            if (result2.Count > 0)
                result.AddRange(result2);

            if (result.Count > 0)
                return Ok(result.DistinctBy(t => t.PageId).OrderBy(t => t.Niveli));
            else
                return NotFound();

        }

        //[HttpGet("GetLinks")]
        //public async Task<IActionResult> GetLinks(string Gjuha = "sq", int LinkTypeID = 1, int PageID = 0, int LayoutId = 0)
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

        //    var linksLista = new List<LinksModel>();

        //    var page = await _unitOfWork.Pages.GetPageById(PageID, gjuhaID);

        //    if (page != null)
        //    {
        //        var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.PageId == PageID, false, new[] { "LinkType" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //        linksLista.AddRange(Lista);
        //    }
        //    if (linksLista.Count == 0 && page != null)
        //    {
        //        var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.LayoutId == page.LayoutId, false, new[] { "LinkType" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //        linksLista.AddRange(Lista);
        //    }
        //    if (linksLista.Count == 0)
        //    {
        //        var result = await _unitOfWork.Links.FindByCondition(t => t.LanguageId == gjuhaID && t.TypeId == LinkTypeID && t.Active == true && t.PageId == null && t.LayoutId == LayoutId, false, new[] { "LinkType" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Link>, List<LinksModel>>(result);
        //        linksLista.AddRange(Lista);
        //    }
        //    return Ok(linksLista.OrderBy(t => t.OrderNo));
        //}

        [HttpGet("GetLinks")]
        public async Task<IActionResult> GetLinks(string Gjuha = "sq", int LinkTypeID = 1, int PageID = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Links.GetLinks(gjuhaID, LinkTypeID, PageID);
           
            return Ok(result);
        }

        //[HttpGet("GetSlides")]
        //public async Task<IActionResult> GetSlides(string Gjuha = "sq", int PageID = 0, int LayoutId = 1)
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var slideLista = new List<SlidesModel>();

        //    var page = await _unitOfWork.Pages.GetPageById(PageID, gjuhaID);
        //    if (page != null)
        //    {
        //        var result = await _unitOfWork.Slide.FindByCondition(t => t.LanguageId == gjuhaID && t.PageId == PageID, false, new[] { "Media", "Layout" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Slide>, List<SlidesModel>>(result);
        //        slideLista.AddRange(Lista);
        //    }
        //    if (slideLista.Count == 0 && page != null)
        //    {
        //        var result = await _unitOfWork.Slide.FindByCondition(t => t.LanguageId == gjuhaID && t.LayoutId == page.LayoutId && t.PageId == null, false, new[] { "Media", "Layout" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Slide>, List<SlidesModel>>(result);
        //        slideLista.AddRange(Lista);
        //    }
        //    if (slideLista.Count == 0)
        //    {
        //        var result = await _unitOfWork.Slide.FindByCondition(t => t.LanguageId == gjuhaID && t.LayoutId == LayoutId && t.PageId == null, false, new[] { "Media", "Layout" }).ToListAsync();
        //        var Lista = _mapper.Map<List<Slide>, List<SlidesModel>>(result);
        //        slideLista.AddRange(Lista);
        //    }

        //    return Ok(slideLista.OrderBy(t => t.OrderNo));

        //}

        [HttpGet("GetSlides")]
        public async Task<IActionResult> GetSlides(string Gjuha = "sq", int PageID = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Slide.GetSlides(gjuhaID, PageID);
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string parameter, string Gjuha = "sq", int skip = 0, int take = 10)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                return NotFound();
            }
            DateTime data = _unitOfWork.BaseConfig.GetDateTime();
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var searchAll = await _unitOfWork.Search.SearchAll(parameter, gjuhaID, skip, take, data);
            int searchAllCount = await _unitOfWork.Search.SearchAllCount(parameter, gjuhaID, data);
            int totalPages = _unitOfWork.BaseConfig.GetTotalPages(searchAllCount, take);

            if (searchAll != null)
                return Ok(new { searchAll, searchAllCount, totalPages });
            else
                return NotFound();
        }

        //[HttpGet("GetSocialNetworks")]
        //public async Task<IActionResult> GetSocialNetworks(string Gjuha = "sq", string Layout = "default", int? ComponentLocationId = null)
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    int? LayoutID = _unitOfWork.BaseConfig.GetLayoutID(Layout);
        //    int _layoutID = LayoutID == null ? 1 : (int)LayoutID;
        //    var result = await _unitOfWork.SocialNetwork.FindByCondition(t => t.LanguageId == gjuhaID && t.LayoutId == _layoutID && t.Active != false &&
        //                                                                (ComponentLocationId != null ? t.ComponentLocationId == ComponentLocationId.Value : true), 
        //                                                                false, new[] { "ComponentLocation", "Layout" }).OrderBy(t => t.OrderNo).ToListAsync();
        //    var lista = _mapper.Map<List<SocialNetwork>, List<SocialNetworksModel>>(result);
        //    return Ok(lista);

        //}
        [HttpGet("GetSocialNetworks")]
        public async Task<IActionResult> GetSocialNetworks(string Gjuha = "sq", string Layout = "default", int? ComponentLocationId = 1)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            int? LayoutID = _unitOfWork.BaseConfig.GetLayoutID(Layout);
            int _layoutID = LayoutID == null ? 1 : (int)LayoutID;
            var result = await _unitOfWork.SocialNetwork.GetSocialNetworks(gjuhaID, _layoutID, ComponentLocationId);

            return Ok(result);

        }

        [HttpGet("GetExtraSettings")]
        public async Task<IActionResult> GetExtraSettings()
        {
            var data = await _unitOfWork.BaseConfig.GetSysSettings();

            var filteredData = data.Where(x => x.Id > 21);

            return Ok(filteredData);
        }

        //[HttpGet("GetPerkthimet")]
        //public async Task<IActionResult> GetPerkthimet(string Gjuha = "sq")
        //{
        //    int gjuhaID = _unitOfWork.BaseConfigurations.GetGjuhaID(Gjuha);

        //    var result = await _unitOfWork.Perkthimet.GetPerkthimet(gjuhaID);

        //    JObject json = JObject.Parse(result);

        //        return Ok(result);
        //}

        //[HttpGet]
        //[Route("GetHomeStatistic")]
        //public async Task<IActionResult> GetHomeStatistic()
        //{
        //    var TreeTypesForLocations = await _unitOfWork.TreeTypesForLocations.FindByCondition(t => t.MunicipalityLocation.Active == true, false, null).ToListAsync();

        //    var nrTotalPlanted = TreeTypesForLocations.Sum(x => x.TotalPlanted);
        //    var nrTotalForPlanting = TreeTypesForLocations.Sum(x => x.TotalForPlanting) - nrTotalPlanted;
        //    var cilesiaAjrit = 0;
        //    var nrLocation = TreeTypesForLocations.Count();

        //    return Ok(new
        //    {
        //        nrTotalPlanted = nrTotalPlanted,
        //        nrTotalForPlanting = nrTotalForPlanting,
        //        cilesiaAjrit = cilesiaAjrit,
        //        nrLocation = nrLocation
        //    });
        //}

        [HttpGet]
        [Route("GetAirQuality")]
        public async Task<IActionResult> GetAirQuality()
        {
            var data = _airQualityServices.GetAirQualityStatistic(); 

            return Ok(data);
        }

        [HttpGet("GetDate")]
        public IActionResult GetDate()
        {
            var data = DateTime.UtcNow;
            return Ok(data);
        }

        [HttpGet]
        [Route("GetResourceTranslationsByTypeAndLanguage")]
        public async Task<IActionResult> GetResourceTranslationsByTypeAndLanguage()
        {
            List<ResourceTranslationTypeByLangDto> list = new List<ResourceTranslationTypeByLangDto>();

            var langList = await _unitOfWork.BaseConfig.GetLangList();
            foreach ( var lang in langList )
            {
                ResourceTranslationTypeByLangDto item = new ResourceTranslationTypeByLangDto();
                item.LangId = lang.Id;
                item.Lang = lang.NameSq;

                var translationType = await _unitOfWork.ResourceTranslation.GetResourceTranslationTypeWithIncludes(lang.Id);
                var type = _mapper.Map<List<ResourceTranslationTypeWithTransDto>?>(translationType);
                item.resourceTranslationType = type;              

                list.Add(item);
            }
            

            
            return Ok(list);
        }


    }
}
