using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface ITagRepository : IGenericRepository<PostTag>
    {
        //Task<IEnumerable<PostTag>> GetAllActiveTagsAsync();
        Task<PostTag?> GetTagsById(int id, int webLangId);
        Task<PagedList<PostTag>> GetPostTagsAsync(FilterParameters parameter);
        Task<bool> AddTagsCollectionInPost(AddTagsCollectionInPost model, List<Language> langList, string UserId);
        Task<bool> RemoveTagsCollectionFromPost(List<int> TagIds, List<Language> langList, bool WebMultiLang, int webLangId, int postId);
    }
}

