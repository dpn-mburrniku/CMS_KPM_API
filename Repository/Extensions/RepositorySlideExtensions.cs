
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
	public static class RepositorySlideExtensions
	{
		public static IQueryable<Slide> FilterSlideByLayout(this IQueryable<Slide> slide, int? filter) =>
		slide.Where(e => e.LayoutId == filter);

		public static IQueryable<Slide> FilterSlideByLanguage(this IQueryable<Slide> slide, int? filter) =>
		slide.Where(e => e.LanguageId == filter);

		public static IQueryable<Slide> FilterIsDeleted(this IQueryable<Slide> slide, bool isDeleted) =>
		slide.Where(e => e.Deleted == isDeleted);
		public static IQueryable<Slide> Search(this IQueryable<Slide> slide, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return slide;

			var lowerCaseTerm = query.Trim().ToLower();

			slide = slide.Where(e => e.Title.ToLower().Contains(lowerCaseTerm) || e.Description.ToLower().Contains(lowerCaseTerm) || e.Link.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm));

			return slide;
		}

		public static IQueryable<Slide> Sort(this IQueryable<Slide> slide, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return slide.OrderBy(e => e.OrderNo);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(Slide).GetProperties(BindingFlags.Public |
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
				return slide.OrderBy(e => e.OrderNo);

			return slide.OrderBy(orderQuery);
		}
	}

}
