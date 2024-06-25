using Entities.Models;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;
using System;
using Abp.Linq.Expressions;

namespace Repository.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }
        public async Task<PagedList<Post>> GetPostsAsync(PostFilterParameters parameter, List<int>? postIds, bool isDeleted)
        {
            DateTime? dtFrom = new();
            DateTime? dtTo = new();
            if (!string.IsNullOrEmpty(parameter.DateFrom))
            {
                try
                {
                    parameter.DateFrom = parameter.DateFrom.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateFrom.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateFrom.Split('/')[1]);
                    int viti = int.Parse(parameter.DateFrom.Split('/')[2]);
                    dtFrom = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtFrom = null;
            }
            if (!string.IsNullOrEmpty(parameter.DateTo))
            {
                try
                {
                    parameter.DateTo = parameter.DateTo.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateTo.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateTo.Split('/')[1]);
                    int viti = int.Parse(parameter.DateTo.Split('/')[2]);
                    dtTo = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtTo = null;
            }

            IQueryable<Post> data = _cmsContext.Posts.Include(x => x.Media).Include(x => x.PostsInCategories).ThenInclude(x => x.PostCategory).ThenInclude(x => x.Layout).IgnoreAutoIncludes().AsNoTracking()
                            .FilterPostByLanguage(parameter.webLangId)
                            .FilterPostByDate(dtFrom, dtTo)
                            .FilterPostByPostCategoryId(postIds)
														.FilterIsDeleted(isDeleted)
														.Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Post>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public async Task<Post> GetPostById(int id, int webLangId)
        {
            var post = await _cmsContext.Posts.FindAsync(id, webLangId);

            return post;
        }

        public async Task<bool> AddMediaCollectionInPost(AddMediaCollectionInPost model, List<Language> langList, string UserId)
        {
            try
            {
                if(model.WebMultiLang)
                {
                    foreach (var item in model.MediaId)
                    {
                        foreach(var lang in langList)
                        {
                            bool existsPostInCurrentLang = _cmsContext.Posts.Where(x => x.Id == model.PostId && x.LanguageId == lang.Id && x.Deleted != true).Any();
                            bool existsMediaInThisPost = _cmsContext.PostMedia.Where(x => x.PostId == model.PostId && x.MediaId == item && x.LanguageId == lang.Id).Any();
                            if (existsPostInCurrentLang && (!existsMediaInThisPost)) //check if posts exists in current language
                            {
                                var postMediaEntity = new PostMedium()
                                {
                                    MediaId = item,
                                    PostId = model.PostId,
                                    LanguageId = lang.Id,
                                    IsSlider = model.IsSlider,
                                    CreatedBy = UserId,
                                    Created = DateTime.Now,
                                };

                                _cmsContext.PostMedia.Add(postMediaEntity);
                                await _cmsContext.SaveChangesAsync();
                            }
                            
                        }
                    }
                }
                else
                {
                    foreach (var item in model.MediaId)
                    {
                        bool existsMediaInThisPost = _cmsContext.PostMedia.Where(x => x.PostId == model.PostId && x.MediaId == item && x.LanguageId == model.webLangId).Any();
                        if (!existsMediaInThisPost)
                        {
                            var postMediaEntity = new PostMedium()
                            {
                                MediaId = item,
                                PostId = model.PostId,
                                LanguageId = model.webLangId,
                                IsSlider = model.IsSlider,
                                CreatedBy = UserId,
                                Created = DateTime.Now,
                            };
                            _cmsContext.PostMedia.Add(postMediaEntity);
                            await _cmsContext.SaveChangesAsync();
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> RemoveMediaCollectionFromPost(List<int> MediaIds, List<Language> langList, bool WebMultiLang, int webLangId, int postId)
        {
            if(MediaIds.Count > 0)
            {
                foreach(int Id in MediaIds)
                {
                    if (WebMultiLang)
                    {
                        foreach (var lang in langList)
                        {
                            var mediaInPost = await _cmsContext.PostMedia.Where(x => x.PostId == postId && x.MediaId == Id && x.LanguageId == lang.Id).FirstOrDefaultAsync();
                            if (mediaInPost != null)
                            {
                                _cmsContext.Remove(mediaInPost);
                                await _cmsContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        var mediaInPost = await _cmsContext.PostMedia.Where(x => x.PostId == postId && x.MediaId == Id && x.LanguageId == webLangId).FirstOrDefaultAsync();
                        if (mediaInPost != null)
                        {
                            _cmsContext.Remove(mediaInPost);
                            await _cmsContext.SaveChangesAsync();
                        }
                    }
                    
                }
            }
            return true;
        }

        public async Task<List<GetPostMediaDto>> GetPostMedia(int postId, int webLangId, bool isSlider)
        {
            var list = await _cmsContext.PostMedia.Include(x => x.Media).ThenInclude(x => x.FileExNavigation).Where(x => x.PostId == postId && x.LanguageId == webLangId && x.IsSlider == isSlider)
                .Select(t => new GetPostMediaDto
                {                    
                    PostId = t.PostId,
                    LanguageId = t.LanguageId,
                    MediaId = t.MediaId,
                    IsSlider = t.IsSlider,
                    OrderNo = t.OrderNo,
                    CreatedBy = t.CreatedBy,
                    Media = t.Media
                }).OrderBy(x=>x.OrderNo).ToListAsync();
            return list;
        }

        public async Task<GetPostMediaDto> GetPostMediaById(int mediaId, int LanguageId)
        {
            var list = await _cmsContext.PostMedia.Include(x => x.Media).Where(x => x.MediaId == mediaId && x.LanguageId == LanguageId)
                .Select(t => new GetPostMediaDto
                {
                    PostId = t.PostId,
                    LanguageId = t.LanguageId,
                    MediaId = t.MediaId,
                    IsSlider = t.IsSlider,
                    OrderNo = t.OrderNo,
                    CreatedBy = t.CreatedBy,
                    Media = t.Media
                }).FirstOrDefaultAsync();
            return list;
        }

        #region web
        public async Task<List<NewsModel>> GetNews(int GjuhaId, List<int> PostimiKategoriaID, int skip, int take, int TitulliLength, int PermbajtjaLength, DateTime? date, string? SearchText, DateTime? DateFrom, DateTime? DateTo, DateTime formatedDateTime)
        {
            var postsInCategoriesQuery = PredicateBuilder.False<PostsInCategory>();

            foreach (var id in PostimiKategoriaID)
            {
                postsInCategoriesQuery = postsInCategoriesQuery.Or(t => t.PostCategoryId == id);
            }

            var postsInCategoriesQueryResult = _cmsContext.PostsInCategories.Where(t => t.LanguageId == GjuhaId).Where(postsInCategoriesQuery);

            var lajmetLista = await (from p in _cmsContext.Posts.Where(t => t.LanguageId == GjuhaId)
                                     join pnk in postsInCategoriesQueryResult on p.Id equals pnk.PostId
                                     join pk in _cmsContext.PostCategories.Where(t => t.LanguageId == GjuhaId) on pnk.PostCategoryId equals pk.Id

                                     where p.Published == true && p.Deleted != true
                                     && (date != null ? p.StartDate >= date && p.StartDate < date.Value.AddDays(1) : true)
                                     && (p.StartDate != null ? p.StartDate <= formatedDateTime : true)
                                     && (p.EndDate != null ? p.EndDate.Value >= formatedDateTime : true)
                                     && (!string.IsNullOrEmpty(SearchText) ? p.Title.Contains(SearchText) : true)
                                     && (DateFrom != null ?
                                         (p.StartDate != null ? p.StartDate.Date >= DateFrom.Value : true) &&
                                         (p.EndDate != null ? p.EndDate.Value.Date >= DateFrom.Value : true) : true)
                                         && (DateTo != null ?
                                         (p.StartDate != null ? p.StartDate.Date <= DateTo.Value : true) : true)

                                     select new NewsModel
                                     {
                                         PostimiId = p.Id,
                                         PostimiTitulli = p.Title.Length > TitulliLength ? p.Title.Substring(0, Math.Min(p.Title.Length, TitulliLength)) + "..." : p.Title,
                                         PostimiPershkrimi = p.Description.Length > PermbajtjaLength ? p.Description.Substring(0, Math.Min(p.Description.Length, PermbajtjaLength)) + "..." : p.Description,
                                         PostimiLokacioni = p.Location,
                                         PostimiAdresa = p.Address,
                                         PostimiDataFillimit = p.StartDate,
                                         PostimiDataInstertimit = p.Created,
                                         PostimiDataMbarimit = p.EndDate,
                                         PostimiDataNgjarjes = p.EventDate,
                                         Publikuar = p.Published,
                                         GjuhaId = p.LanguageId,
                                         Url = pk.PageId != null && pk.PageId > 0 ? _cmsContext.Pages.Include(x => x.Layout).Where(t => t.Id == pk.PageId && t.LanguageId == pk.LanguageId).FirstOrDefault().Layout.Path + "/NewsDetails/" + pk.PageId + "/" + p.Id : "/NewsDetails/" + _cmsContext.Pages.Where(t => t.TemplateId == 5 && t.LanguageId == pk.LanguageId).FirstOrDefault().Id + "/" + p.Id,
                                         media = (from m in _cmsContext.Media
                                                  where m.Id == p.MediaId
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
                                                  }).FirstOrDefault(),
                                         docs = (from pm in _cmsContext.PostMedia
                                                 join m in _cmsContext.Media on pm.MediaId equals m.Id
                                                 where pm.PostId == p.Id && pm.IsSlider == false
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
                                                     OtherSource = m.OtherSourceLink,
                                                     OrderNo = pm.OrderNo
                                                 }).OrderBy(x=>x.OrderNo).FirstOrDefault(),
                                     }
                                   ).Distinct().OrderByDescending(t => t.PostimiDataFillimit).Skip(skip).Take(take).ToListAsync();

            return lajmetLista;
        }

        public async Task<int> GetNewsCount(int GjuhaId, List<int> PostimiKategoriaID, DateTime? date, string? SearchText, DateTime? DateFrom, DateTime? DateTo, DateTime formatedDateTime)
        {
            var postsInCategoriesQuery = PredicateBuilder.False<PostsInCategory>();

            foreach (var id in PostimiKategoriaID)
            {
                postsInCategoriesQuery = postsInCategoriesQuery.Or(t => t.PostCategoryId == id);
            }

            var postsInCategoriesQueryResult = _cmsContext.PostsInCategories.Where(t => t.LanguageId == GjuhaId).Where(postsInCategoriesQuery);

            var totalRowsCount = await (from p in _cmsContext.Posts.Where(t => t.LanguageId == GjuhaId)
                                        join pnk in postsInCategoriesQueryResult on p.Id equals pnk.PostId
                                        join pk in _cmsContext.PostCategories.Where(t => t.LanguageId == GjuhaId) on pnk.PostCategoryId equals pk.Id

                                        where p.Published == true && p.Deleted != true
                                         && (date != null ? p.StartDate >= date && p.StartDate < date.Value.AddDays(1) : true)
                                         && (p.StartDate != null ? p.StartDate <= formatedDateTime : true)
                                         && (p.EndDate != null ? p.EndDate.Value >= formatedDateTime : true)
                                         && (!string.IsNullOrEmpty(SearchText) ? p.Title.Contains(SearchText) : true)
                                         && (DateFrom != null ?
                                             (p.StartDate != null ? p.StartDate.Date >= DateFrom.Value : true) &&
                                             (p.EndDate != null ? p.EndDate.Value.Date >= DateFrom.Value : true) : true)
                                             && (DateTo != null ?
                                             (p.StartDate != null ? p.StartDate.Date <= DateTo.Value : true) : true)

                                        select new
                                        {
                                            p.Id
                                        }).Distinct().CountAsync();

            return totalRowsCount;
        }

        public async Task<NewsModel> GetNewsDetails(int GjuhaId, int PostimiID)
        {

            var postimi = await (from p in _cmsContext.Posts.Where(t => t.Id == PostimiID && t.LanguageId == GjuhaId)                                 
                                 select new NewsModel
                                 {
                                     PostimiId = p.Id,
                                     PostimiTitulli = p.Title,
                                     PostimiPershkrimi = p.Description,
                                     PostimiPermbajtja = p.Content,
                                     PostimiLokacioni = p.Location,
                                     PostimiAdresa = p.Address,
                                     PostimiDataInstertimit = p.Created,
                                     PostimiDataFillimit = p.StartDate,
                                     PostimiDataMbarimit = p.EndDate,
                                     PostimiDataNgjarjes = p.EventDate,
                                     Publikuar = p.Published,
                                     GjuhaId = p.LanguageId,
                                     media = (from m in _cmsContext.Media
                                              where m.Id == p.MediaId
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
                                 }).FirstOrDefaultAsync();

            return postimi;
        }

        public async Task<List<MediaModel>> GetNewsDetailsMedia(int GjuhaId, int PostimiID, bool isSlider)
        {

            var postimimedia = await (from p in _cmsContext.Posts
                                      join pm in _cmsContext.PostMedia.Where(t => t.LanguageId == GjuhaId && t.IsSlider == isSlider) on p.Id equals pm.PostId
                                      join m in _cmsContext.Media on pm.MediaId equals m.Id
                                      where p.Id == PostimiID && p.LanguageId == GjuhaId && p.Deleted != true
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
                                          OtherSource = m.OtherSourceLink,
                                          IsSlider = pm.IsSlider,
                                          OrderNo = pm.OrderNo
                                      }).OrderBy(x=>x.OrderNo).ToListAsync();

            return postimimedia;
        }

        public async Task<List<NewsCategoriesModel>> GetNewsCategories(int PageId, int gjuhaID, bool GetAll)
        {
            var NewsCategories = await (from c in _cmsContext.PostCategories
                                        where c.Active == true && c.LanguageId == gjuhaID
                                              && (PageId > 0 ? c.PageId == PageId : true) && (GetAll ? true : c.ShowInFilters == true)

                                        select new NewsCategoriesModel
                                        {
                                            PostimetKategoriaId = c.Id,
                                            PostimetKategoriaPershkrimi = c.Title,
                                            PageId = c.PageId.Value
                                        }).ToListAsync();
            return NewsCategories;
        }

        public async Task<int> GetCheckNewsForThisDate(int gjuhaID, DateTime? date)
        {
            int countNews = _cmsContext.Posts.Where(c => c.LanguageId == gjuhaID && c.Deleted != true && c.StartDate >= date.Value.Date
                                                            && c.StartDate < date.Value.Date.AddDays(1)).Count();

            return countNews;
        }
        #endregion
    }
}
