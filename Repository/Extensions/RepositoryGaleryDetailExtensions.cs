
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
	public static class RepositoryGaleryDetailExtensions
	{
		public static IQueryable<GaleryDetail> FilterGaleryDetailByMedia(this IQueryable<GaleryDetail> galeryDetail, int? filter) =>
		galeryDetail.Where(e => filter > 0 ? e.MediaId == filter : true);
		public static IQueryable<GaleryDetail> FilterGaleryDetailByHeader(this IQueryable<GaleryDetail> galeryDetail, int? filter) =>
		galeryDetail.Where(e => filter > 0 ? e.HeaderId == filter : true);
		public static IQueryable<GaleryDetail> FilterGaleryDetailByLanguage(this IQueryable<GaleryDetail> galeryDetail, int? filter) =>
		galeryDetail.Where(e => e.LanguageId == filter);

		public static IQueryable<GaleryDetail> Search(this IQueryable<GaleryDetail> galeryDetail, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return galeryDetail;

			var lowerCaseTerm = query.Trim().ToLower();

			galeryDetail = galeryDetail.Where(e => e.GaleryHeader.Title.ToLower().Contains(lowerCaseTerm));

			return galeryDetail;
		}

		public static IQueryable<GaleryDetail> Sort(this IQueryable<GaleryDetail> galeryDetail, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return galeryDetail.OrderBy(e => e.OrderNo);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(GaleryDetail).GetProperties(BindingFlags.Public |
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
				return galeryDetail.OrderBy(e => e.OrderNo);

			return galeryDetail.OrderBy(orderQuery);
		}
	}

}
