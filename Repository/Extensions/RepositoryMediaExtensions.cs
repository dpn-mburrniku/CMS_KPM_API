using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
	public static class RepositoryMediaExtensions
	{
		public static IQueryable<Medium> FilterMediaByMediaExCategory(this IQueryable<Medium> media, int? filter) =>
		media.Where(e => filter > 0 ? e.MediaExCategoryId == filter : true);
		public static IQueryable<Medium> FilterMediaByCreateDate(this IQueryable<Medium> media, DateTime? datefrom, DateTime? dateTo) =>
		media.Where(e => (datefrom != null ? e.Created >= datefrom : true) && (dateTo != null ? e.Created <= dateTo : true));

		public static IQueryable<Medium> Search(this IQueryable<Medium> media, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return media;

			var lowerCaseTerm = query.Trim().ToLower();

			media = media.Where(e => e.Name.ToLower().Contains(lowerCaseTerm) || e.FileName.ToString().ToLower().Contains(lowerCaseTerm));


			return media;
		}

		public static IQueryable<Medium> Sort(this IQueryable<Medium> media, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return media.OrderByDescending(e => e.Created);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(Medium).GetProperties(BindingFlags.Public |
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
				return media.OrderBy(e => e.Name);

			return media.OrderBy(orderQuery);
		}
	}

}
