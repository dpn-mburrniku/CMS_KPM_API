using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class SlideRepostitory : GenericRepository<Slide>, ISlideRepository
    {
        public SlideRepostitory(CmsContext cmsContext
           ) : base(cmsContext)
        {

        }

        public async Task<PagedList<Slide>> GetSlideAsync(FilterParameters parameter, bool isDeleted)
        {
            IQueryable<Slide> data = _cmsContext.Slides.Include(x => x.Layout).Include(x => x.Page).Include(x => x.Media).IgnoreAutoIncludes().AsNoTracking()
                            .FilterSlideByLanguage(parameter.webLangId)
                            .FilterSlideByLayout(parameter.LayoutId)
                            .FilterIsDeleted(isDeleted)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);

            return PagedList<Slide>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);            
        }

        public async Task<Slide> GetSlideById(int id, int webLangId)
        {
            var slide = await _cmsContext.Slides.Include(x => x.Media).Where(x => x.Id == id && x.LanguageId == webLangId).FirstOrDefaultAsync();

            return slide;

        }

        #region web
        public async Task<List<SlidesModel>> GetSlides(int GjuhaId, int PageID)
        {
            var slideLista = new List<SlidesModel>();

            var page = _cmsContext.Pages.Where(t => t.Id == PageID && t.LanguageId == GjuhaId).FirstOrDefault();
            if (page != null)
            {
                slideLista = await (from s in _cmsContext.Slides
                                    where s.LanguageId == GjuhaId
                                    && s.PageId == PageID && s.Deleted == false
                                    select new SlidesModel
                                    {
                                        SllideId = s.Id,
                                        SllideTitulli = s.Title,
                                        SllidePershkrimi = s.Description,
                                        SllideDataInsertimit = s.Created,
                                        Linku = s.Link,
                                        OrderNr = s.OrderNo,
                                        GjuhaId = s.LanguageId,
                                        MediaId = s.MediaId,
                                        media = (from m in _cmsContext.Media
                                                 where m.Id == s.MediaId
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
                                    }).OrderBy(t => t.OrderNr).ToListAsync();
            }

            if (slideLista.Count == 0 && page != null)
            {
                slideLista = await (from s in _cmsContext.Slides
                                    where s.LanguageId == GjuhaId
                                    && s.LayoutId == page.LayoutId && s.PageId == null && s.Deleted == false
                                    select new SlidesModel
                                    {
                                        SllideId = s.Id,
                                        SllideTitulli = s.Title,
                                        SllidePershkrimi = s.Description,
                                        SllideDataInsertimit = s.Created,
                                        Linku = s.Link,
                                        OrderNr = s.OrderNo,
                                        GjuhaId = s.LanguageId,
                                        MediaId = s.MediaId,
                                        media = (from m in _cmsContext.Media
                                                 where m.Id == s.MediaId
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
                                    }).OrderBy(t => t.OrderNr).ToListAsync();
            }

            if (slideLista.Count == 0)
            {
                slideLista = await (from s in _cmsContext.Slides
                                    where s.LanguageId == GjuhaId && s.PageId == null && s.LayoutId == 1 && s.Deleted == false
                                    select new SlidesModel
                                    {
                                        SllideId = s.Id,
                                        SllideTitulli = s.Title,
                                        SllidePershkrimi = s.Description,
                                        SllideDataInsertimit = s.Created,
                                        Linku = s.Link,
                                        OrderNr = s.OrderNo,
                                        GjuhaId = s.LanguageId,
                                        MediaId = s.MediaId,
                                        media = (from m in _cmsContext.Media
                                                 where m.Id == s.MediaId
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
                                    }).OrderBy(t => t.OrderNr).ToListAsync();
            }

            return slideLista;
        }
        #endregion
    }
}
