using Entities.Models;
using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
	public class PostCategoryRepository : GenericRepository<PostCategory>, IPostCategoryRepository
	{
		public PostCategoryRepository(CmsContext cmsContext) : base(cmsContext)
		{

		}

		public async Task<PostCategory?> GetPostCaregoriesById(int id, int webLangId)
		{
			var postCategory = await _cmsContext.PostCategories.Include(x => x.Layout).Where(x => x.Id == id && x.LanguageId == webLangId).FirstOrDefaultAsync();

			return postCategory;
		}

		public async Task<PagedList<PostCategory>> GetPostCategoriesAsync(PostCategoryFilterParameters parameter)
		{
			IQueryable<PostCategory> data = _cmsContext.PostCategories.Include(x => x.Layout).Include(x => x.Page).IgnoreAutoIncludes().AsNoTracking()
										 .FilterPostCategoryByLanguage(parameter.webLangId)
										 .FilterPostCategoryByLayout(parameter.LayoutId)
										 .FilterPostCategoryByPage(parameter.PageId)
										 .Search(parameter.Query)
										 .Sort(parameter.Sort.key + " " + parameter.Sort.order);
			//.ToListAsync();

			return PagedList<PostCategory>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}
	}
}
