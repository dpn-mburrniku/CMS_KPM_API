using Entities.Models;
using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects;
using System.Runtime.InteropServices;

namespace Repository.Repositories
{
    public class TagRepository : GenericRepository<PostTag>, ITagRepository
    {

        public TagRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<PagedList<PostTag>> GetPostTagsAsync(FilterParameters parameter)
        {
            IQueryable<PostTag> data = _cmsContext.PostTags
                            .FilterTagByLanguage(parameter.webLangId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<PostTag>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public async Task<PostTag?> GetTagsById(int id, int webLangId)
        {
            var tags = await _cmsContext.PostTags.FindAsync(id, webLangId);
            return tags;
        }

        public async Task<bool> AddTagsCollectionInPost(AddTagsCollectionInPost model, List<Language> langList, string UserId)
        {
            try
            {
                if (model.WebMultiLang)
                {
                    foreach (var item in model.PostTagId)
                    {
                        foreach (var lang in langList)
                        {
                            bool existsPostInCurrentLang = _cmsContext.Posts.Where(x => x.Id == model.PostId && x.LanguageId == lang.Id).Any();
                            if (existsPostInCurrentLang) //check if posts exists in current language
                            {
                                var postInTagEntity = new PostsInTag()
                                {
                                    PostTagId = item,
                                    PostId = model.PostId,
                                    LanguageId = lang.Id,
                                    CreatedBy = UserId,
                                    Created = DateTime.Now,
                                };

                                _cmsContext.PostsInTags.Add(postInTagEntity);
                                await _cmsContext.SaveChangesAsync();
                            }

                        }
                    }
                }
                else
                {
                    foreach (var item in model.PostTagId)
                    {
                        var postTagsEntity = new PostsInTag()
                        {                           
                            PostTagId = item,
                            PostId = model.PostId,
                            LanguageId = model.LanguageId,
                            CreatedBy = UserId,
                            Created = DateTime.Now,
                        }; 
                        _cmsContext.PostsInTags.Add(postTagsEntity);
                        await _cmsContext.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> RemoveTagsCollectionFromPost(List<int> TagIds, List<Language> langList, bool WebMultiLang, int webLangId, int postId)
        {
            if (TagIds.Count > 0)
            {
                foreach (int Id in TagIds)
                {
                    if (WebMultiLang)
                    {
                        foreach (var lang in langList)
                        {
                            var tagInPost = await _cmsContext.PostsInTags.Where(x => x.PostId == postId && x.PostTagId == Id && x.LanguageId == lang.Id).FirstOrDefaultAsync();
                            if (tagInPost != null)
                            {
                                _cmsContext.Remove(tagInPost);
                                await _cmsContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        var tagInPost = await _cmsContext.PostsInTags.Where(x => x.PostId == postId && x.PostTagId == Id && x.LanguageId == webLangId).FirstOrDefaultAsync();
                        if (tagInPost != null)
                        {
                            _cmsContext.Remove(tagInPost);
                            await _cmsContext.SaveChangesAsync();
                        }
                    }

                }
            }
            return true;
        }



    }
}