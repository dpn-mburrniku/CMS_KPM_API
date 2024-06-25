using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class SearchRepository : GenericRepository<Personel>, ISearchRepository
    {
        public SearchRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<List<SearchModel>> SearchAll(string parameter, int GjuhaID, int skip, int take, DateTime formatedDateTime)
        {
            List<SearchModel> searchModels = new List<SearchModel>();

            var Page = await (from m in _cmsContext.Menus
                              join p in _cmsContext.Pages on m.PageId equals p.Id

                              join t in _cmsContext.Templates on p.TemplateId equals t.Id into temp
                              from template in temp.DefaultIfEmpty()

                              where p.LanguageId == GjuhaID && m.LanguageId == GjuhaID && p.Deleted != true
                              && p.PageName.Contains(parameter)
                              select new SearchModel
                              {
                                  ID = m.PageId,
                                  Name = p.PageName,
                                  CategoryName = (GjuhaID == 2 ? "Menus" : (GjuhaID == 3 ? "Menu" : "Menytë")),
                                  CategoryId = 1,
                                  OtherSource = template.TemplateUrl == null ? true : false,
                                  Url = template.TemplateUrl == null ? m.OtherSourceUrl :
                                  (m.IsRedirect != true ? template.TemplateUrl + (template.TemplateUrlWithId == true ? "/" + m.PageId : "") :
                                       (from m1 in _cmsContext.Menus
                                        join p1 in _cmsContext.Pages on m1.PageId equals p1.Id

                                        join t1 in _cmsContext.Templates on p1.TemplateId equals t1.Id into temp1
                                        from template1 in temp1.DefaultIfEmpty()

                                        where p1.Id == m.PageIdredirect && p1.LanguageId == m.LanguageId
                                        select template1.TemplateUrl == null ? m1.OtherSourceUrl : template1.TemplateUrl + (template1.TemplateUrlWithId == true ? "/" + m1.PageId : "")).FirstOrDefault()
                                       )
                              }).ToListAsync();

            if (Page.Count > 0)
            {
                searchModels.AddRange(Page);
            }

            var Docs = await (from pm in _cmsContext.PageMedia

                              join m in _cmsContext.Media on pm.MediaId equals m.Id

                              where pm.LanguageId == GjuhaID && pm.IsSlider == false
                                      && (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
                                      && (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
                                      && pm.Name.Contains(parameter)
                              select new SearchModel
                              {
                                  ID = pm.MediaId,
                                  Name = pm.Name,
                                  CategoryName = (GjuhaID == 2 ? "Documents" : (GjuhaID == 3 ? "Dokumenti" : "Dokumentet")),
                                  CategoryId = 2,
                                  OtherSource = m.IsOtherSource,
                                  Url = m.IsOtherSource ? m.OtherSourceLink : m.FileName.ToString() + m.FileEx,
                              }).ToListAsync();

            if (Docs.Count > 0)
            {
                searchModels.AddRange(Docs);
            }

            var lajmetLista = await (from p in _cmsContext.Posts.Where(t => t.LanguageId == GjuhaID)
                                     join pnk in _cmsContext.PostsInCategories.Where(t => t.LanguageId == GjuhaID) on p.Id equals pnk.PostId
                                     join pk in _cmsContext.PostCategories.Where(t => t.LanguageId == GjuhaID) on pnk.PostCategoryId equals pk.Id
                                     where p.Published == true && p.Deleted != true && p.LanguageId == GjuhaID
                                     && (p.StartDate <= formatedDateTime)
                                     && (p.EndDate != null ? p.EndDate.Value >= formatedDateTime: true)
                                     && p.Title.Contains(parameter)

                                     select new SearchModel
                                     {
                                         ID = p.Id,
                                         Name = p.Title.Length > 200 ? p.Title.Substring(0, Math.Min(p.Title.Length, 200)) + "..." : p.Title,
                                         CategoryName = (GjuhaID == 2 ? "Posts" : (GjuhaID == 3 ? "Postovi" : "Postimet")),
                                         CategoryId = 3,
                                         Url = pk.PageId != null && pk.PageId > 0 ? _cmsContext.Pages.Include(x => x.Layout).Where(t => t.Id == pk.PageId).FirstOrDefault().Layout.Path + "/NewsDetails/" + pk.PageId + "/" + p.Id 
                                                                                  : "/NewsDetails/" + _cmsContext.Pages.Where(t => t.TemplateId == 5).FirstOrDefault().Id + "/" + p.Id,
                                     }).Distinct().ToListAsync();

            if (lajmetLista.Count > 0)
            {
                searchModels.AddRange(lajmetLista);
            }

            return searchModels.OrderBy(t => t.Name).Skip(skip).Take(take).ToList();
        }

        public async Task<int> SearchAllCount(string parameter, int GjuhaID, DateTime formatedDateTime)
        {
            int allRows = 0;
            int Page = await (from m in _cmsContext.Menus
                              join p in _cmsContext.Pages on m.PageId equals p.Id

                              join t in _cmsContext.Templates on p.TemplateId equals t.Id into temp
                              from template in temp.DefaultIfEmpty()

                              where p.LanguageId == GjuhaID && m.LanguageId == GjuhaID && p.Deleted != true
                              && p.PageName.Contains(parameter)
                              select new SearchModel
                              {
                                  ID = m.PageId
                              }).CountAsync();

            int Docs = await (from pm in _cmsContext.PageMedia

                              join m in _cmsContext.Media on pm.MediaId equals m.Id

                              where pm.LanguageId == GjuhaID
                                      && (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
                                      && (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
                                      && pm.Name.Contains(parameter)
                              select new SearchModel
                              {
                                  ID = pm.MediaId,
                              }).CountAsync();


            int lajmetLista = await (from p in _cmsContext.Posts.Where(t => t.LanguageId == GjuhaID)
                                     join pnk in _cmsContext.PostsInCategories.Where(t => t.LanguageId == GjuhaID) on p.Id equals pnk.PostId
                                     join pk in _cmsContext.PostCategories.Where(t => t.LanguageId == GjuhaID) on pnk.PostCategoryId equals pk.Id
                                     where p.Published == true && p.Deleted != true && p.LanguageId == GjuhaID
                                     && (p.StartDate <= formatedDateTime)
                                     && (p.EndDate != null ? p.EndDate.Value >= formatedDateTime : true)
                                     && p.Title.Contains(parameter)

                                     select new SearchModel
                                     {
                                         ID = p.Id,
                                     }).Distinct().CountAsync();


            allRows = Page + Docs + lajmetLista;

            return allRows;
        }
    }
}
