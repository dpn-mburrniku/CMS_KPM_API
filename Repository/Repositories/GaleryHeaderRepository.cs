using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
	public class GaleryHeaderRepository : GenericRepository<GaleryHeader>, IGaleryHeaderRepository
	{

		public GaleryHeaderRepository(CmsContext cmsContext) : base(cmsContext)
		{
			
		}

		public async Task<GaleryHeader?> GetGaleryHeaderById(int id, int webLangId)
		{
			var galeryHeader = await _cmsContext.GaleryHeaders.FindAsync(id, webLangId);

			return galeryHeader;
		}

		public async Task<PagedList<GaleryHeader>> GetGaleryHeaderAsync(GaleryFilterParameters parameter, bool isDeleted)
		{
            IQueryable<GaleryHeader> data = _cmsContext.GaleryHeaders.Include(x => x.Layout).Include(x => x.GaleryDetails).IgnoreAutoIncludes().AsNoTracking()
                                            .FilterFaqHeaderByLanguage(parameter.webLangId)
                                            .FilterFaqHeaderByCategory(parameter.GaleryCategoryId)
                                            .FilterFaqHeaderByLayout(parameter.LayoutId)
                                            .FilterIsDeleted(isDeleted)
                                            .Search(parameter.Query)
                                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
											//.ToListAsync();

			return PagedList<GaleryHeader>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}

        #region web
        public async Task<int> GetPageId(int CategoryId, string? Layout)
        {
            int pageId = 0;

            if (CategoryId == 1)
            {
                var page = (from t in _cmsContext.Templates
                            join p in _cmsContext.Pages.Include(x => x.Layout) on t.Id equals p.TemplateId
                            where t.TemplateUrl == "/PhotoGalery"
                            && (p.Layout.NameSq.ToLower() == Layout || p.Layout.NameEn.ToLower() == Layout || p.Layout.NameSr.ToLower() == Layout)
                            select new
                            {
                                p.Id
                            }).FirstOrDefault();
                if (page != null)
                {
                    pageId = page.Id;
                }
            }
            else
            {
                var page = (from t in _cmsContext.Templates
                            join p in _cmsContext.Pages.Include(x => x.Layout) on t.Id equals p.TemplateId
                            where t.TemplateUrl == "/VideoGalery"
                            && (p.Layout.NameSq.ToLower() == Layout || p.Layout.NameEn.ToLower() == Layout || p.Layout.NameSr.ToLower() == Layout)
                            select new
                            {
                                p.Id
                            }).FirstOrDefault();
                if (page != null)
                {
                    pageId = page.Id;
                }
            }

            return pageId;
        }

        public async Task<List<GaleryModel>> GetGaleries(int gjuhaID, int CategoryId, int? LayoutID, int? PageID, int skip, int take)
        {
            var galeriaLista = await (from g in _cmsContext.GaleryHeaders.Include(t => t.GaleryDetails)

                                      where g.CategoryId == CategoryId && g.IsDeleted != true
                                      && g.GaleryDetails.Count > 0
                                      && g.LanguageId == gjuhaID
                                      && (LayoutID != null ? g.LayoutId == LayoutID : true)
                                      && (take == 1 ? g.ShfaqNeHome == true : true)
                                      select new GaleryModel
                                      {
                                          MediaGaleriaId = g.Id,
                                          MediaGaleriaPershkrimi = g.Title,
                                          MediaGaleriaKategoriaId = g.CategoryId,
                                          OrderNr = g.OrderNo,
                                          Fshire = g.IsDeleted,
                                          Url = LayoutID != null && LayoutID != 1 ? _cmsContext.Layouts.Where(t => t.Id == LayoutID).FirstOrDefault().Path +
                                          (g.CategoryId == 1 ? "/PhotoGaleryDetails/" : "/VideoGaleryDetails/") + PageID + "/" + g.Id :
                                          (g.CategoryId == 1 ? "/PhotoGaleryDetails/" : "/VideoGaleryDetails/") + PageID + "/" + g.Id,
                                          media = (from m in _cmsContext.Media
                                                   where m.Id == g.GaleryDetails.OrderBy(t => t.OrderNo).FirstOrDefault().MediaId
                                                   select new MediaModel
                                                   {
                                                       MediaId = m.Id,
                                                       MediaEmri = m.FileName.ToString(),
                                                       MediaEmri_medium = m.FileNameMedium,
                                                       MediaEmri_small = m.FileNameSmall,
                                                       MediaEx = m.FileEx,
                                                       MediaPershkrimi = m.Name,
                                                       MediaDataInsertimit = m.Created,
                                                       MediaExKategoriaId = m.MediaExCategoryId,
                                                       IsOtherSource = m.IsOtherSource,
                                                       OtherSource = m.OtherSourceLink
                                                   }).FirstOrDefault()
                                      }).OrderBy(t => t.OrderNr).Skip(skip).Take(take).ToListAsync();

            return galeriaLista;
        }

        public async Task<int> GetGaleriesCount(int gjuhaID, int CategoryId, int? LayoutID, int? PageID)
        {
            int pageCount = await (from g in _cmsContext.GaleryHeaders.Include(t => t.GaleryDetails)

                                   where g.CategoryId == CategoryId && g.IsDeleted != true
                                      && g.GaleryDetails.Count > 0
                                   && g.LanguageId == gjuhaID
                                   && (LayoutID != null ? g.LayoutId == LayoutID : true)
                                   select new GaleryModel
                                   {
                                       MediaGaleriaId = g.Id,
                                       MediaGaleriaPershkrimi = g.Title,
                                       MediaGaleriaKategoriaId = g.CategoryId,
                                       OrderNr = g.OrderNo,
                                   }).CountAsync();

            return pageCount;
        }
        #endregion
    }
}
