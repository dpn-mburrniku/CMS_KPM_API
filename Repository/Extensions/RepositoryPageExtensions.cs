
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;
using System.Security.Cryptography.X509Certificates;

namespace Repository.Extensions
{
	public static class RepositoryPageExtensions
	{
		public static IQueryable<Page> FilterPageByLayout(this IQueryable<Page> page, int? filter) =>
		//page.Where(e => filter > 0 ? e.LayoutId == filter : layoutIds.Contains(e.LayoutId));
		page.Where(e => e.LayoutId == filter);
		public static IQueryable<Page> FilterPageByLanguage(this IQueryable<Page> page, int? filter) =>
		page.Where(e => e.LanguageId == filter);

		public static IQueryable<Page> FilterPageByParentId(this IQueryable<Page> page, int? filter) =>
		page.Where(e => e.PageParentId == filter);

		public static IQueryable<Page> FilterPageByTemplateId(this IQueryable<Page> page, int? filter) =>
		page.Where(e => e.TemplateId == (filter != null ? filter : e.TemplateId));
		public static IQueryable<Page> FilterPageDeleted(this IQueryable<Page> page, bool filter) =>
		page.Where(e => e.Deleted == filter);

		public static IQueryable<Page> FilterIsSubPage(this IQueryable<Page> page) =>
			 page.Where(e => e.IsSubPage == true);

		public static IQueryable<Page> FilterSubPageByParentId(this IQueryable<Page> page, int? filter) =>
		page.Where(e => filter > 0 ? e.PageParentId == filter : true);
		public static IQueryable<Page> Search(this IQueryable<Page> page, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return page;

			var lowerCaseTerm = query.Trim().ToLower();

			page = page.Where(e => e.Id.ToString().Contains(lowerCaseTerm) || e.PageName.ToLower().Contains(lowerCaseTerm));

			return page;
		}

		public static IQueryable<Page> Sort(this IQueryable<Page> page, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return page.OrderByDescending(e => e.Id);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(Page).GetProperties(BindingFlags.Public |
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
				return page.OrderByDescending(e => e.Id);

			return page.OrderBy(orderQuery);
		}
	}

}
