using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Repository.Extensions
{
    public static class RepositoryPostCategoriesExtensions
    {
        public static IQueryable<PostCategory> FilterPostCategoryByLayout(this IQueryable<PostCategory> postCategory, int? filter) =>
        postCategory.Where(e => e.LayoutId == filter);
		    public static IQueryable<PostCategory> FilterPostCategoryByPage(this IQueryable<PostCategory> postCategory, int? filter) =>
        postCategory.Where(e => filter > 0 ? e.PageId == filter : true);
        public static IQueryable<PostCategory> FilterPostCategoryByLanguage(this IQueryable<PostCategory> postCategory, int? filter) =>
        postCategory.Where(e => e.LanguageId == filter);

        public static IQueryable<PostCategory> Search(this IQueryable<PostCategory> postCategory, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return postCategory;

            var lowerCaseTerm = query.Trim().ToLower();

            postCategory = postCategory.Where(e => e.Title.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm) || e.ShowInFilters.ToString().Contains(lowerCaseTerm));

            return postCategory;
        }

        public static IQueryable<PostCategory> Sort(this IQueryable<PostCategory> postCategory, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return postCategory.OrderBy(e => e.Id);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(PostCategory).GetProperties(BindingFlags.Public |
                        BindingFlags.Instance);

            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>

                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            if (string.IsNullOrWhiteSpace(orderQuery))
                return postCategory.OrderBy(e => e.Id);

            return postCategory.OrderBy(orderQuery);
        }
    }
}
