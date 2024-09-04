using Entities.Models;
using CMS.API;
using Entities.Models;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {

        }
        public async Task<bool> AddCollectionPagesInMenu(AddCollectionMenu model, string userId)
        {
            try
            {
                foreach (var item in model.PageId)
                {
                    int maxID = 0;
                    try
                    {
                        maxID = _cmsContext.Menus.Max(x => x.Id);
                    }
                    catch (Exception ex)
                    {
                    }
                    maxID += 1;
                    var pageList = await _cmsContext.Pages.Where(x => x.Id == item && x.Deleted != true).ToListAsync();
                    foreach (var page in pageList)
                    {
                        var checkIfExist = await _cmsContext.Menus.Where(x => x.MenuTypeId == model.MenuTypeId && x.PageId == page.Id).AnyAsync();

                        if (!checkIfExist)
                        {
                            var menuEntity = new Menu();

                            menuEntity.Id = maxID;
                            menuEntity.LanguageId = page.LanguageId;
                            menuEntity.PageId = page.Id;
                            menuEntity.MenuTypeId = model.MenuTypeId;
                            menuEntity.OrderNo = 0;
                            menuEntity.Level = 1;
                            menuEntity.Active = true;
                            menuEntity.IsClickable = true;
                            menuEntity.IsMegaMenu = false;
                            menuEntity.IsRedirect = false;
                            menuEntity.Created = DateTime.Now;
                            menuEntity.CreatedBy = userId;
                            _cmsContext.Menus.Add(menuEntity);
                        }
                    }

                    await _cmsContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Menu?> GetMenuById(int id, int webLangId)
        {
            var data = await _cmsContext.Menus.FindAsync(id, webLangId);

            return data;
        }

        #region WebMenus
        public async Task<List<MenuModel>> GetSideBarMenus(int gjuhaID, int LocationMenuID, int? ParentMenuID, int ParentPageID)
        {
            ParentMenuID = ParentMenuID == 0 ? null : ParentMenuID;
            var menuLista = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Media)

                                   where m.Active != false && (m.Page != null ? m.Page.Deleted != true : true) && m.MenuTypeId == LocationMenuID &&
                                   m.LanguageId == gjuhaID && m.MenuParentId == ParentMenuID &&
                                   (ParentPageID > 0 ? m.PageParentId == ParentPageID : m.PageParentId == null || m.PageParentId == 0)
                                   select new MenuModel
                                   {
                                       PageId = m.PageId,
                                       MenuId = m.Id,
                                       MenuPrindId = m.MenuParentId,
                                       MenuLocationId = m.MenuTypeId,
                                       MenuLocationName = m.MenuType.Title,
                                       PageName = m.IsOtherSource ? m.OtherSourceName : m.Page.PageName,
                                       PagePrindId = m.PageParentId,
                                       NrOrder = m.OrderNo,
                                       Niveli = 1,
                                       hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                       OtherSource = m.IsOtherSource,
                                       Targeti = m.Target,
                                       IsMegaMenu = m.IsMegaMenu,
                                       IsClicked = m.IsClickable,
                                       Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                       (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                               (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)
                                                where m1.PageId == m.PageIdredirect && (m1.Page != null ? m1.Page.Deleted != true : true) && m1.LanguageId == m.LanguageId
                                                select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                               ),
                                       submenu = (from cm in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Media)
                                                  where cm.Active == true && (cm.Page != null ? cm.Page.Deleted != true : true) && cm.MenuTypeId == LocationMenuID &&
                                                  cm.LanguageId == gjuhaID && cm.MenuParentId == m.Id &&
                                                  (ParentPageID > 0 ? cm.PageParentId == ParentPageID : cm.PageParentId == null || cm.PageParentId == 0)
                                                  select new MenuModel
                                                  {
                                                      PageId = cm.PageId,
                                                      MenuId = cm.Id,
                                                      MenuPrindId = cm.MenuParentId,
                                                      MenuLocationId = cm.MenuTypeId,
                                                      MenuLocationName = cm.MenuType.Title,
                                                      PageName = cm.IsOtherSource ? cm.OtherSourceName : cm.Page.PageName,
                                                      PagePrindId = m.PageParentId,
                                                      NrOrder = cm.OrderNo,
                                                      Niveli = 1,
                                                      hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == cm.Id).Count() > 0 ? true : false,
                                                      OtherSource = cm.IsOtherSource,
                                                      Targeti = cm.Target,
                                                      IsMegaMenu = cm.IsMegaMenu,
                                                      IsClicked = cm.IsClickable,
                                                      Url = cm.IsOtherSource == true ? cm.OtherSourceUrl :
                                                           (cm.IsRedirect != true ? cm.Page.Layout.Path + cm.Page.Template.TemplateUrl + (cm.Page.Template.TemplateUrlWithId == true ? "/" + cm.PageId : "") :
                                                                   (from cm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                  .Include(t => t.Page)
                                                                                                       .ThenInclude(t => t.Layout)
                                                                                                  .Include(t => t.Page)
                                                                                                        .ThenInclude(t => t.Template)
                                                                    where cm1.PageId == cm.PageIdredirect && (cm1.Page != null ? cm1.Page.Deleted != true : true) && cm1.LanguageId == cm.LanguageId
                                                                    select cm1.IsOtherSource == true ? cm1.OtherSourceUrl : cm1.Page.Layout.Path + cm1.Page.Template.TemplateUrl + (cm1.Page.Template.TemplateUrlWithId == true ? "/" + cm1.PageId : "")).FirstOrDefault()
                                                                   ),
                                                      submenu = (from scm in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                              .Include(t => t.Page)
                                                                                                   .ThenInclude(t => t.Layout)
                                                                                              .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Template)
                                                                                              .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Media)
                                                                 where scm.Active == true && (scm.Page != null ? scm.Page.Deleted != true : true) && scm.MenuTypeId == LocationMenuID &&
                                                                 scm.LanguageId == gjuhaID && scm.MenuParentId == cm.Id &&
                                                                 (ParentPageID > 0 ? scm.PageParentId == ParentPageID : scm.PageParentId == null || scm.PageParentId == 0)
                                                                 select new MenuModel
                                                                 {
                                                                     PageId = scm.PageId,
                                                                     MenuId = scm.Id,
                                                                     MenuPrindId = scm.MenuParentId,
                                                                     MenuLocationId = scm.MenuTypeId,
                                                                     MenuLocationName = scm.MenuType.Title,
                                                                     PageName = scm.IsOtherSource ? scm.OtherSourceName : scm.Page.PageName,
                                                                     PagePrindId = m.PageParentId,
                                                                     NrOrder = scm.OrderNo,
                                                                     Niveli = 1,
                                                                     hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == scm.Id).Count() > 0 ? true : false,
                                                                     OtherSource = scm.IsOtherSource,
                                                                     Targeti = scm.Target,
                                                                     IsMegaMenu = scm.IsMegaMenu,
                                                                     IsClicked = scm.IsClickable,
                                                                     Url = scm.IsOtherSource == true ? scm.OtherSourceUrl :
                                                                           (scm.IsRedirect != true ? scm.Page.Layout.Path + scm.Page.Template.TemplateUrl + (scm.Page.Template.TemplateUrlWithId == true ? "/" + scm.PageId : "") :
                                                                                   (from scm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                                  .Include(t => t.Page)
                                                                                                                       .ThenInclude(t => t.Layout)
                                                                                                                  .Include(t => t.Page)
                                                                                                                        .ThenInclude(t => t.Template)
                                                                                    where scm1.PageId == scm.PageIdredirect && (scm1.Page != null ? scm1.Page.Deleted != true : true) && scm1.LanguageId == scm.LanguageId
                                                                                    select scm1.IsOtherSource == true ? scm1.OtherSourceUrl : scm1.Page.Layout.Path + scm1.Page.Template.TemplateUrl + (scm1.Page.Template.TemplateUrlWithId == true ? "/" + scm1.PageId : "")).FirstOrDefault()
                                                                                   ),
                                                                     submenu = (from sscm in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                              .Include(t => t.Page)
                                                                                                                   .ThenInclude(t => t.Layout)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Template)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Media)
                                                                                where sscm.Active == true && (sscm.Page != null ? sscm.Page.Deleted != true : true) && scm.MenuTypeId == LocationMenuID &&
                                                                                sscm.LanguageId == gjuhaID && sscm.MenuParentId == scm.Id &&
                                                                                (ParentPageID > 0 ? sscm.PageParentId == ParentPageID : sscm.PageParentId == null || sscm.PageParentId == 0)
                                                                                select new MenuModel
                                                                                {
                                                                                    PageId = sscm.PageId,
                                                                                    MenuId = sscm.Id,
                                                                                    MenuPrindId = sscm.MenuParentId,
                                                                                    MenuLocationId = sscm.MenuTypeId,
                                                                                    MenuLocationName = sscm.MenuType.Title,
                                                                                    PageName = sscm.IsOtherSource ? sscm.OtherSourceName : sscm.Page.PageName,
                                                                                    PagePrindId = m.PageParentId,
                                                                                    NrOrder = sscm.OrderNo,
                                                                                    Niveli = 1,
                                                                                    hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == sscm.Id).Count() > 0 ? true : false,
                                                                                    OtherSource = sscm.IsOtherSource,
                                                                                    Targeti = sscm.Target,
                                                                                    IsMegaMenu = sscm.IsMegaMenu,
                                                                                    IsClicked = sscm.IsClickable,
                                                                                    Url = sscm.IsOtherSource == true ? sscm.OtherSourceUrl :
                                                                                          (sscm.IsRedirect != true ? sscm.Page.Layout.Path + sscm.Page.Template.TemplateUrl + (sscm.Page.Template.TemplateUrlWithId == true ? "/" + sscm.PageId : "") :
                                                                                                   (from sscm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                                                  .Include(t => t.Page)
                                                                                                                                       .ThenInclude(t => t.Layout)
                                                                                                                                  .Include(t => t.Page)
                                                                                                                                        .ThenInclude(t => t.Template)
                                                                                                    where sscm1.PageId == sscm.PageIdredirect && (sscm1.Page != null ? sscm1.Page.Deleted != true : true) && sscm1.LanguageId == sscm.LanguageId
                                                                                                    select sscm1.IsOtherSource == true ? sscm1.OtherSourceUrl : sscm1.Page.Layout.Path + sscm1.Page.Template.TemplateUrl + (sscm1.Page.Template.TemplateUrlWithId == true ? "/" + sscm1.PageId : "")).FirstOrDefault()
                                                                                                   ),
                                                                                    media = sscm.Page.Media
                                                                                }).OrderBy(x => x.NrOrder).ToList(),
                                                                     media = scm.Page.Media
                                                                 }).OrderBy(x => x.NrOrder).ToList(),
                                                      media = cm.Page.Media
                                                  }).OrderBy(x => x.NrOrder).ToList(),
                                       media = m.Page.Media
                                   }).OrderBy(x => x.NrOrder).ToListAsync();

            return menuLista;
        }

        public async Task<List<MenuModel>> GetMenus(int gjuhaID, int LocationMenuID, int? ParentMenuID)
        {
            ParentMenuID = ParentMenuID == 0 ? null : ParentMenuID;
            var menuLista = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Media)
                                   where m.Active != false && (m.Page != null ? m.Page.Deleted != true : true) && m.MenuTypeId == LocationMenuID &&
                                   m.LanguageId == gjuhaID && m.MenuParentId == ParentMenuID
                                   select new MenuModel
                                   {
                                       PageId = m.PageId,
                                       MenuId = m.Id,
                                       MenuPrindId = m.MenuParentId,
                                       MenuLocationId = m.MenuTypeId,
                                       MenuLocationName = m.MenuType.Title,
                                       PageName = m.IsOtherSource ? m.OtherSourceName : m.Page.PageName,
                                       PageText = m.Page.PageText,
                                       PagePrindId = m.PageParentId,
                                       NrOrder = m.OrderNo,
                                       Niveli = 1,
                                       hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                       OtherSource = m.IsOtherSource,
                                       Targeti = m.Target,
                                       IsMegaMenu = m.IsMegaMenu,
                                       IsClicked = m.IsClickable,
                                       Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                       (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                               (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Layout)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Template)
                                                where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId && (m1.Page != null ? m1.Page.Deleted != true : true)
                                                select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                               ),
                                       submenu = (from cm in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                .Include(t => t.Page)
                                                                                    .ThenInclude(t => t.Layout)
                                                                                .Include(t => t.Page)
                                                                                    .ThenInclude(t => t.Template)
                                                                                .Include(t => t.Page)
                                                                                    .ThenInclude(t => t.Media)
                                                  where cm.Active == true && (cm.Page != null ? cm.Page.Deleted != true : true) && cm.MenuTypeId == LocationMenuID &&
                                                  cm.LanguageId == gjuhaID && cm.MenuParentId == m.Id
                                                  select new MenuModel
                                                  {
                                                      PageId = cm.PageId,
                                                      MenuId = cm.Id,
                                                      MenuPrindId = cm.MenuParentId,
                                                      MenuLocationId = cm.MenuTypeId,
                                                      MenuLocationName = cm.MenuType.Title,
                                                      PageName = cm.IsOtherSource ? cm.OtherSourceName : cm.Page.PageName,
                                                      PageText = cm.Page.PageText,
                                                      PagePrindId = m.PageParentId,
                                                      NrOrder = cm.OrderNo,
                                                      Niveli = 1,
                                                      hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == cm.Id).Count() > 0 ? true : false,
                                                      OtherSource = cm.IsOtherSource,
                                                      Targeti = cm.Target,
                                                      IsMegaMenu = cm.IsMegaMenu,
                                                      IsClicked = cm.IsClickable,
                                                      Url = cm.IsOtherSource == true ? cm.OtherSourceUrl :
                                                           (cm.IsRedirect != true ? cm.Page.Layout.Path + cm.Page.Template.TemplateUrl + (cm.Page.Template.TemplateUrlWithId == true ? "/" + cm.PageId : "") :
                                                                   (from cm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Layout)
                                                                                                .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Template)
                                                                    where cm1.PageId == cm.PageIdredirect && (cm1.Page != null ? cm1.Page.Deleted != true : true) && cm1.LanguageId == cm.LanguageId
                                                                    select cm1.IsOtherSource == true ? cm1.OtherSourceUrl : cm1.Page.Layout.Path + cm1.Page.Template.TemplateUrl + (cm1.Page.Template.TemplateUrlWithId == true ? "/" + cm1.PageId : "")).FirstOrDefault()
                                                                   ),
                                                      submenu = (from scm in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                              .Include(t => t.Page)
                                                                                                   .ThenInclude(t => t.Layout)
                                                                                              .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Template)
                                                                                              .Include(t => t.Page)
                                                                                                    .ThenInclude(t => t.Media)
                                                                 where scm.Active == true && (scm.Page != null ? scm.Page.Deleted != true : true) && scm.MenuTypeId == LocationMenuID &&
                                                                 scm.LanguageId == gjuhaID && scm.MenuParentId == cm.Id
                                                                 select new MenuModel
                                                                 {
                                                                     PageId = scm.PageId,
                                                                     MenuId = scm.Id,
                                                                     MenuPrindId = scm.MenuParentId,
                                                                     MenuLocationId = scm.MenuTypeId,
                                                                     MenuLocationName = scm.MenuType.Title,
                                                                     PageName = scm.IsOtherSource ? scm.OtherSourceName : scm.Page.PageName,
                                                                     PageText = scm.Page.PageText,
                                                                     PagePrindId = m.PageParentId,
                                                                     NrOrder = scm.OrderNo,
                                                                     Niveli = 1,
                                                                     hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == scm.Id).Count() > 0 ? true : false,
                                                                     OtherSource = scm.IsOtherSource,
                                                                     Targeti = scm.Target,
                                                                     IsMegaMenu = scm.IsMegaMenu,
                                                                     IsClicked = scm.IsClickable,
                                                                     Url = scm.IsOtherSource == true ? scm.OtherSourceUrl :
                                                                           (scm.IsRedirect != true ? scm.Page.Layout.Path + scm.Page.Template.TemplateUrl + (scm.Page.Template.TemplateUrlWithId == true ? "/" + scm.PageId : "") :
                                                                                   (from scm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                              .Include(t => t.Page)
                                                                                                                   .ThenInclude(t => t.Layout)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Template)
                                                                                    where scm1.PageId == scm.PageIdredirect && (scm1.Page != null ? scm1.Page.Deleted != true : true) && scm1.LanguageId == scm.LanguageId
                                                                                    select scm1.IsOtherSource == true ? scm1.OtherSourceUrl : scm1.Page.Layout.Path + scm1.Page.Template.TemplateUrl + (scm1.Page.Template.TemplateUrlWithId == true ? "/" + scm1.PageId : "")).FirstOrDefault()
                                                                                   ),
                                                                     submenu = (from sscm in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                              .Include(t => t.Page)
                                                                                                                   .ThenInclude(t => t.Layout)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Template)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Media)
                                                                                where sscm.Active == true && (sscm.Page != null ? sscm.Page.Deleted != true : true) && scm.MenuTypeId == LocationMenuID &&
                                                                                sscm.LanguageId == gjuhaID && sscm.MenuParentId == scm.Id
                                                                                select new MenuModel
                                                                                {
                                                                                    PageId = sscm.PageId,
                                                                                    MenuId = sscm.Id,
                                                                                    MenuPrindId = sscm.MenuParentId,
                                                                                    MenuLocationId = sscm.MenuTypeId,
                                                                                    MenuLocationName = sscm.MenuType.Title,
                                                                                    PageName = sscm.IsOtherSource ? sscm.OtherSourceName : sscm.Page.PageName,
                                                                                    PageText = sscm.Page.PageText,
                                                                                    PagePrindId = m.PageParentId,
                                                                                    NrOrder = sscm.OrderNo,
                                                                                    Niveli = 1,
                                                                                    hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == sscm.Id).Count() > 0 ? true : false,
                                                                                    OtherSource = sscm.IsOtherSource,
                                                                                    Targeti = sscm.Target,
                                                                                    IsMegaMenu = sscm.IsMegaMenu,
                                                                                    IsClicked = sscm.IsClickable,
                                                                                    Url = sscm.IsOtherSource == true ? sscm.OtherSourceUrl :
                                                                                          (sscm.IsRedirect != true ? sscm.Page.Layout.Path + sscm.Page.Template.TemplateUrl + (sscm.Page.Template.TemplateUrlWithId == true ? "/" + sscm.PageId : "") :
                                                                                                   (from sscm1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                                              .Include(t => t.Page)
                                                                                                                   .ThenInclude(t => t.Layout)
                                                                                                              .Include(t => t.Page)
                                                                                                                    .ThenInclude(t => t.Template)
                                                                                                    where sscm1.PageId == sscm.PageIdredirect &&
                                                                                                    (sscm1.Page != null ? sscm1.Page.Deleted != true : true) &&
                                                                                                    sscm1.LanguageId == sscm.LanguageId
                                                                                                    select sscm1.IsOtherSource == true ? sscm1.OtherSourceUrl : sscm1.Page.Layout.Path + sscm1.Page.Template.TemplateUrl + (sscm1.Page.Template.TemplateUrlWithId == true ? "/" + sscm1.PageId : "")).FirstOrDefault()
                                                                                                   ),
                                                                                    media = sscm.Page.Media
                                                                                }).OrderBy(x => x.NrOrder).ToList(),
                                                                     media = scm.Page.Media
                                                                 }).OrderBy(x => x.NrOrder).ToList(),
                                                      media = cm.Page.Media
                                                  }).OrderBy(x => x.NrOrder).ToList(),
                                       media = m.Page.Media
                                   }).OrderBy(x => x.NrOrder).ToListAsync();

            return menuLista;
        }

        public async Task<List<MenuModel>> GetMenusWithoutChilds(int gjuhaID, int LocationMenuID, int? ParentMenuID)
        {
            ParentMenuID = ParentMenuID == 0 ? null : ParentMenuID;
            var menuLista = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Media)

                                   where m.Active != false && (m.Page != null ? m.Page.Deleted != true : true) && m.MenuTypeId == LocationMenuID &&
                                   m.LanguageId == gjuhaID && m.MenuParentId == ParentMenuID
                                   select new MenuModel
                                   {
                                       PageId = m.PageId,
                                       MenuId = m.Id,
                                       MenuPrindId = m.MenuParentId,
                                       MenuLocationId = m.MenuTypeId,
                                       MenuLocationName = m.MenuType.Title,
                                       PageName = m.IsOtherSource ? m.OtherSourceName : m.Page.PageName,
                                       PagePrindId = m.PageParentId,
                                       NrOrder = m.OrderNo,
                                       Niveli = 1,
                                       hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                       OtherSource = m.IsOtherSource,
                                       Targeti = m.Target,
                                       IsMegaMenu = m.IsMegaMenu,
                                       IsClicked = m.IsClickable,
                                       Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                       (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                               (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Layout)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Template)
                                                where m1.PageId == m.PageIdredirect && (m1.Page != null ? m1.Page.Deleted != true : true) && m1.LanguageId == m.LanguageId
                                                select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                               ),
                                       media = m.Page.Media
                                   }).OrderBy(x => x.NrOrder).ToListAsync();

            return menuLista;
        }

        public async Task<List<MenuAtoZModel>> GetMenusAtoZ(int gjuhaID)
        {
            var menuList = new List<MenuAtoZModel>();

            //var getAllMenus = getMenusStartWithLetter(gjuhaID);

            for (char c = 'A'; c < 'Z'; c++)
            {
                var menu = new MenuAtoZModel();
                menu.Letter = c.ToString();
                menu.menus = getMenusStartWithLetter(gjuhaID).Where(t => t.PageName.StartsWith(c.ToString())).Distinct().OrderBy(x => x.PageName).ToList();

                menuList.Add(menu);
            }

            return menuList;
        }

        public async Task<List<MenuAtoZModel>> GetMenusAtoZWithLetter(int gjuhaID, string Letter)
        {
            var menuList = new List<MenuAtoZModel>();

            //var getAllMenus = getMenusStartWithLetter(gjuhaID);

            var menu = new MenuAtoZModel();
            menu.Letter = Letter;
            menu.menus = getMenusStartWithLetter(gjuhaID).Where(t => t.PageName.StartsWith(Letter)).Distinct().OrderBy(x => x.PageName).ToList();

            menuList.Add(menu);

            return menuList;
        }

        private IQueryable<MenuModel> getMenusStartWithLetter(int gjuhaID)
        {
            IQueryable<MenuModel> menu = from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)

                                         where m.Active != false && (m.Page != null ? m.Page.Deleted != true : true) && m.LanguageId == gjuhaID && m.IsClickable != false
                                         select new MenuModel
                                         {
                                             PageId = m.PageId,
                                             PageName = m.IsOtherSource ? m.OtherSourceName :
                                                        (m.MenuType.LayoutId > 1 ? (m.LanguageId == 1 ? m.Page.Layout.NameSq :
                                                                           (m.LanguageId == 3 ? m.Page.Layout.NameSr : m.Page.Layout.NameEn)) + " - " : "")
                                                        + m.Page.PageName,
                                             OtherSource = m.IsOtherSource,
                                             Targeti = m.Target,
                                             Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                  (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                          (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                       .Include(t => t.Page)
                                                                                           .ThenInclude(t => t.Layout)
                                                                                       .Include(t => t.Page)
                                                                                           .ThenInclude(t => t.Template)
                                                           where m1.PageId == m.PageIdredirect && (m1.Page != null ? m1.Page.Deleted != true : true) && m1.LanguageId == m.LanguageId
                                                           select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                          ),
                                         };

            return menu;
        }

        public async Task<List<MenuModel>> GetMenuPath(int PageID, int gjuhaID, int LocationMenuID, int niveli)
        {
            int parentMenuId = 0;
            int superParentMenuId = 0;

            var _page = _cmsContext.Pages.Where(x => x.Id == PageID).FirstOrDefault();

            Layout? layout = new Layout();

            if (_page != null)
            {
                layout = await _cmsContext.Layouts.FindAsync(_page.LayoutId);
            }

            if (layout != null)
            {
                var menulocation = (from g in _cmsContext.MenuTypes
                                    where g.LayoutId == layout.Id
                                    select g.Id
                                        ).ToList();

                if (menulocation != null)
                {
                    var pagemenu = _cmsContext.Menus.Where(x => x.PageId == PageID && menulocation.Contains((int)x.MenuTypeId)).FirstOrDefault();

                    if (pagemenu != null)
                    {
                        LocationMenuID = (int)pagemenu.MenuTypeId;
                    }
                    else
                    {
                        LocationMenuID = menulocation.FirstOrDefault();
                    }
                }
            }

            var menutypes = _cmsContext.MenuTypes.Where(x => x.Id == LocationMenuID).FirstOrDefault();
            int MenuLocationGroupID = menutypes != null ? menutypes.Id : 0;

            var selectedmenu = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)

                                      where m.PageId == PageID && m.LanguageId == gjuhaID
                                      && m.MenuTypeId == LocationMenuID && m.Page.LayoutId == layout.Id
                                      select new MenuModel
                                      {
                                          LayoutID = m.Page.LayoutId,
                                          PageId = m.PageId,
                                          MenuId = m.Id,
                                          MenuPrindId = m.MenuParentId,
                                          MenuLocationId = m.MenuTypeId,
                                          MenuLocationName = m.MenuType.Title,
                                          PageName = m.Page.PageName,
                                          PagePrindId = m.PageParentId,
                                          NrOrder = m.OrderNo,
                                          Niveli = niveli + 4,
                                          hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                          OtherSource = m.IsOtherSource,
                                          Targeti = m.Target,
                                          IsMegaMenu = m.IsMegaMenu,
                                          IsClicked = m.IsClickable,
                                          Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                               (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Layout)
                                                                            .Include(t => t.Page)
                                                                                .ThenInclude(t => t.Template)
                                                where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                               )
                                      }).OrderBy(x => x.NrOrder).ToListAsync();
            if (selectedmenu.Count() > 0)
            {
                var parentselectedmenu = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)

                                                where m.MenuTypeId == LocationMenuID
                                                      && m.Id == selectedmenu.FirstOrDefault().MenuPrindId
                                                      && m.LanguageId == gjuhaID
                                                      && m.Page.LayoutId == layout.Id
                                                select new MenuModel
                                                {
                                                    LayoutID = m.Page.LayoutId,
                                                    PageId = m.PageId,
                                                    MenuId = m.Id,
                                                    MenuPrindId = m.MenuParentId,
                                                    MenuLocationId = m.MenuTypeId,
                                                    MenuLocationName = m.MenuType.Title,
                                                    PageName = m.Page.PageName,
                                                    PagePrindId = m.PageParentId,
                                                    NrOrder = m.OrderNo,
                                                    Niveli = niveli + 3,
                                                    hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                                    OtherSource = m.IsOtherSource,
                                                    Targeti = m.Target,
                                                    IsMegaMenu = m.IsMegaMenu,
                                                    IsClicked = m.IsClickable,
                                                    Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                    (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                       (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                    .Include(t => t.Page)
                                                                                        .ThenInclude(t => t.Layout)
                                                                                    .Include(t => t.Page)
                                                                                        .ThenInclude(t => t.Template)
                                                        where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                        select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                       )
                                                }).OrderBy(x => x.NrOrder).ToListAsync();

                if (parentselectedmenu.Count > 0)
                {
                    selectedmenu.AddRange(parentselectedmenu);
                    parentMenuId = parentselectedmenu.FirstOrDefault().MenuPrindId != null ? (int)parentselectedmenu.FirstOrDefault().MenuPrindId : 0;
                }
            }

            if (parentMenuId > 0)
            {
                var secondparentselectedmenu = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Layout)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Template)


                                                      where m.MenuTypeId == LocationMenuID
                                                          && m.Id == parentMenuId
                                                          && m.LanguageId == gjuhaID
                                                         && m.Page.LayoutId == layout.Id
                                                      select new MenuModel
                                                      {
                                                          LayoutID = m.Page.LayoutId,
                                                          PageId = m.PageId,
                                                          MenuId = m.Id,
                                                          MenuPrindId = m.MenuParentId,
                                                          MenuLocationId = m.MenuTypeId,
                                                          MenuLocationName = m.MenuType.Title,
                                                          PageName = m.Page.PageName,
                                                          PagePrindId = m.PageParentId,
                                                          NrOrder = m.OrderNo,
                                                          Niveli = niveli + 2,
                                                          hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                                          OtherSource = m.IsOtherSource,
                                                          Targeti = m.Target,
                                                          IsMegaMenu = m.IsMegaMenu,
                                                          IsClicked = m.IsClickable,
                                                          Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                            (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Layout)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Template)
                                                             where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                             select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                            )
                                                      }).OrderBy(x => x.NrOrder).ToListAsync();

                if (secondparentselectedmenu.Count > 0)
                {
                    selectedmenu.AddRange(secondparentselectedmenu);
                    superParentMenuId = secondparentselectedmenu.FirstOrDefault().MenuPrindId != null ? (int)secondparentselectedmenu.FirstOrDefault().MenuPrindId : 0;
                }
            }

            if (superParentMenuId > 0)
            {
                var thirdparentselectedmenu = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Layout)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Template)

                                                     where m.MenuTypeId == LocationMenuID
                                                         && m.Id == superParentMenuId
                                                          && m.LanguageId == gjuhaID
                                                         && m.Page.LayoutId == layout.Id
                                                     select new MenuModel
                                                     {
                                                         LayoutID = m.Page.LayoutId,
                                                         PageId = m.PageId,
                                                         MenuId = m.Id,
                                                         MenuPrindId = m.MenuParentId,
                                                         MenuLocationId = m.MenuTypeId,
                                                         MenuLocationName = m.MenuType.Title,
                                                         PageName = m.Page.PageName,
                                                         PagePrindId = m.PageParentId,
                                                         NrOrder = m.OrderNo,
                                                         Niveli = niveli + 1,
                                                         hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                                         OtherSource = m.IsOtherSource,
                                                         Targeti = m.Target,
                                                         IsMegaMenu = m.IsMegaMenu,
                                                         IsClicked = m.IsClickable,
                                                         Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                            (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Layout)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Template)
                                                             where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                             select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                            )
                                                     }).OrderBy(x => x.NrOrder).ToListAsync();

                if (thirdparentselectedmenu.Count > 0)
                {
                    selectedmenu.AddRange(thirdparentselectedmenu);
                }
            }
            return selectedmenu;
        }

        public async Task<List<MenuModel>> GetMenuPathLayoutHome(int LayoutID, int gjuhaID, int LocationMenuID, int niveli)
        {
            int parentMenuId = 0;
            int superParentMenuId = 0;

            Layout? layout = await _cmsContext.Layouts.FindAsync(LayoutID);

            var menutypes = _cmsContext.MenuTypes.Where(x => x.Id == LocationMenuID).FirstOrDefault();
            int MenuLocationGroupID = menutypes != null ? menutypes.Id : 0;

            var selectedmenu = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Layout)
                                                                                 .Include(t => t.Page)
                                                                                      .ThenInclude(t => t.Template)

                                      where m.Page.LayoutId == LayoutID && m.LanguageId == gjuhaID
                                      && m.MenuTypeId == LocationMenuID
                                      select new MenuModel
                                      {
                                          LayoutID = LayoutID,
                                          PageId = m.PageId,
                                          MenuId = m.Id,
                                          MenuPrindId = m.MenuParentId,
                                          MenuLocationId = m.MenuTypeId,
                                          MenuLocationName = m.MenuType.Title,
                                          PageName = m.Page.PageName,
                                          PagePrindId = m.PageParentId,
                                          NrOrder = m.OrderNo,
                                          Niveli = 1,
                                          hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                          OtherSource = m.IsOtherSource,
                                          Targeti = m.Target,
                                          IsMegaMenu = m.IsMegaMenu,
                                          IsClicked = m.IsClickable,
                                          Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                            (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Layout)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Template)
                                                             where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                             select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                            )
                                      }).OrderBy(x => x.NrOrder).ToListAsync();


            return selectedmenu;

        }

        public async Task<List<MenuModel>> GetMenuByPageID(int PageID, int gjuhaID)
        {
            var menuLista = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)

                                   where m.LanguageId == gjuhaID && m.PageId == PageID
                                   select new MenuModel
                                   {
                                       LayoutID = m.Page.LayoutId,
                                       PageId = m.PageId,
                                       MenuId = m.Id,
                                       MenuPrindId = m.MenuParentId,
                                       MenuLocationId = m.MenuTypeId,
                                       MenuLocationName = m.MenuType.Title,
                                       PageName = m.Page.PageName,
                                       PagePrindId = m.PageParentId,
                                       NrOrder = m.OrderNo,
                                       Niveli = 1,
                                       hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                       OtherSource = m.IsOtherSource,
                                       Targeti = m.Target,
                                       IsMegaMenu = m.IsMegaMenu,
                                       IsClicked = m.IsClickable,
                                       Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                            (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Layout)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Template)
                                                             where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                             select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                            )
                                   }).OrderBy(x => x.NrOrder).ToListAsync();

            return menuLista;
        }

        public async Task<List<MenuModel>> GetMenuByPageID(int PageID, int gjuhaID, int LocationMenuID)
        {
            var menuLista = await (from m in _cmsContext.Menus.Include(t => t.MenuType)
                                                              .Include(t => t.Page)
                                                                   .ThenInclude(t => t.Layout)
                                                              .Include(t => t.Page)
                                                                    .ThenInclude(t => t.Template)

                                   where m.LanguageId == gjuhaID && m.PageId == PageID && m.MenuTypeId == LocationMenuID
                                   select new MenuModel
                                   {
                                       PageId = m.PageId,
                                       MenuId = m.Id,
                                       MenuPrindId = m.MenuParentId,
                                       MenuLocationId = m.MenuTypeId,
                                       MenuLocationName = m.MenuType.Title,
                                       PageName = m.Page.PageName,
                                       PagePrindId = m.PageParentId,
                                       NrOrder = m.OrderNo,
                                       Niveli = 1,
                                       hasChild = _cmsContext.Menus.Where(t => t.MenuParentId == m.Id).Count() > 0 ? true : false,
                                       OtherSource = m.IsOtherSource,
                                       Targeti = m.Target,
                                       IsMegaMenu = m.IsMegaMenu,
                                       IsClicked = m.IsClickable,
                                       Url = m.IsOtherSource == true ? m.OtherSourceUrl :
                                                          (m.IsRedirect != true ? m.Page.Layout.Path + m.Page.Template.TemplateUrl + (m.Page.Template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                                            (from m1 in _cmsContext.Menus.Include(t => t.MenuType)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Layout)
                                                                                        .Include(t => t.Page)
                                                                                            .ThenInclude(t => t.Template)
                                                             where m1.PageId == m.PageIdredirect && m1.LanguageId == m.LanguageId
                                                             select m1.IsOtherSource == true ? m1.OtherSourceUrl : m1.Page.Layout.Path + m1.Page.Template.TemplateUrl + (m1.Page.Template.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                                            )
                                   }).OrderBy(x => x.NrOrder).ToListAsync();

            return menuLista;
        }
        #endregion
    }
}
