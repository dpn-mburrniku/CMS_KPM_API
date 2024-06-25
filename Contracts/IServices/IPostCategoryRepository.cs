using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public   interface IPostCategoryRepository : IGenericRepository<PostCategory>
    {
        Task<PostCategory?> GetPostCaregoriesById(int id, int webLangId);
        Task<PagedList<PostCategory>> GetPostCategoriesAsync(PostCategoryFilterParameters parameter);
    }
}
