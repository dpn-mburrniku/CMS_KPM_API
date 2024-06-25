using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
	public class GaleryDetailRepository : GenericRepository<GaleryDetail>, IGaleryDetailRepository
	{
		public GaleryDetailRepository (CmsContext cmsContext) : base(cmsContext)
		{

		}

        public async Task<GaleryDetail> GetMediaById(int id, int langId, int headerId)
        {
            var media = await _cmsContext.GaleryDetails.Where(x => x.MediaId == id && x.LanguageId == langId && x.HeaderId == headerId).FirstOrDefaultAsync();

            return media;
        }

        public async Task<GaleryDetail?> GetGaleryDetailById(int id, int webLangId)
		{
			var galeryDetail = await _cmsContext.GaleryDetails.FindAsync(id, webLangId);
			
			return galeryDetail;
		}

		public async Task<PagedList<GaleryDetail>> GetGaleryDetailAsync(GaleryDetailParameters parameter)
		{
            IQueryable<GaleryDetail> data =  _cmsContext.GaleryDetails.Include(x => x.GaleryHeader).IgnoreAutoIncludes().AsNoTracking()
                                            .FilterGaleryDetailByLanguage(parameter.webLangId)
                                            .FilterGaleryDetailByHeader(parameter.GaleryHeaderId)
                                            .FilterGaleryDetailByMedia(parameter.GaleryMediaId)
                                            .Search(parameter.Query)
                                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
											//.ToListAsync();

			return PagedList<GaleryDetail>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}

        #region web
        public async Task<List<GaleryDetailsModel>> GetGaleryDetails(int MediaGaleriaID, int LanguageId)
        {
            //int[] DetajetIDs = _appContext.CmsMediaGaleriaDetajets.Where(t => t.MediaGaleriaId == MediaGaleriaID).Select(t => t.MediaId).ToArray();

            var galeriaLista = await (from g in _cmsContext.GaleryHeaders
                                      where g.Id == MediaGaleriaID && g.LanguageId == LanguageId
                                      select new GaleryDetailsModel
                                      {
                                          MediaGaleriaId = g.Id,
                                          MediaGaleriaPershkrimi = g.Title,
                                          MediaGaleriaKategoriaId = g.CategoryId,
                                          OrderNr = g.OrderNo,
                                          Fshire = g.IsDeleted,
                                          media = (from gd in _cmsContext.GaleryDetails.OrderBy(t => t.OrderNo)
                                                   join m in _cmsContext.Media on gd.MediaId equals m.Id
                                                   where gd.HeaderId == g.Id && gd.LanguageId == LanguageId
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
                                                   }).ToList()
                                      }).OrderBy(t => t.OrderNr).ToListAsync();

            return galeriaLista;
        }
        #endregion
    }
}
