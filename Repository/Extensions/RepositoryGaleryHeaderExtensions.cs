using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
	public static class RepositoryGaleryHeaderExtensions
	{
		public static IQueryable<GaleryHeader> FilterFaqHeaderByLayout(this IQueryable<GaleryHeader> GaleryHeader, int? filter) =>
				//GaleryHeader.Where(e => filter > 0 ? e.LayoutId == filter : layoutIds.Contains(e.LayoutId));
				GaleryHeader.Where(e => e.LayoutId == filter);
		public static IQueryable<GaleryHeader> FilterFaqHeaderByLanguage(this IQueryable<GaleryHeader> GaleryHeader, int? filter) =>
GaleryHeader.Where(e => e.LanguageId == filter);
		public static IQueryable<GaleryHeader> FilterFaqHeaderByCategory(this IQueryable<GaleryHeader> GaleryHeader, int? filter) =>
		GaleryHeader.Where(e => e.CategoryId == filter);

		public static IQueryable<GaleryHeader> FilterIsDeleted(this IQueryable<GaleryHeader> GaleryHeader, bool isDeleted) =>
			 GaleryHeader.Where(e => e.IsDeleted == isDeleted);

		public static IQueryable<GaleryHeader> Search(this IQueryable<GaleryHeader> GaleryHeader, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return GaleryHeader;

			var lowerCaseTerm = query.Trim().ToLower();

			GaleryHeader = GaleryHeader.Where(e => e.Title.ToLower().Contains(lowerCaseTerm));

			return GaleryHeader;
		}

		public static IQueryable<GaleryHeader> Sort(this IQueryable<GaleryHeader> GaleryHeader, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return GaleryHeader.OrderBy(e => e.OrderNo);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(GaleryHeader).GetProperties(BindingFlags.Public |
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
				return GaleryHeader.OrderBy(e => e.OrderNo);

			return GaleryHeader.OrderBy(orderQuery);
		}
	}

}
