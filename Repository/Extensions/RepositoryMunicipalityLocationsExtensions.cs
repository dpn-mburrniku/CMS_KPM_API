using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
	public static class RepositoryMunicipalityLocationsExtensions
	{
		public static IQueryable<MunicipalityLocation> FilterMunicipalityLocations(this IQueryable<MunicipalityLocation> municipalityLocation, int? filter) =>
		municipalityLocation.Where(e => filter > 0 ? e.MunicipalityId == filter : true);

		public static IQueryable<MunicipalityLocation> FilterMeasureUnits(this IQueryable<MunicipalityLocation> municipalityLocation, int? filter) =>
		municipalityLocation.Where(e => filter > 0 ? e.MeasureUnitId == filter : true);
		public static IQueryable<MunicipalityLocation> Search(this IQueryable<MunicipalityLocation> municipalityLocation, string query, int? webLangId)
		{
			if (string.IsNullOrWhiteSpace(query))
				return municipalityLocation;

            var lowerCaseTerm = query.Trim().ToLower();

			if (webLangId == 2)
				municipalityLocation = municipalityLocation.Where(e => e.NameEn.ToLower().Contains(lowerCaseTerm)
			|| e.Municipality.NameEn.ToLower().Contains(lowerCaseTerm)
            || e.Area.ToString().Contains(lowerCaseTerm)
            );
			else if (webLangId == 3)
				municipalityLocation = municipalityLocation.Where(e => e.NameSr.ToLower().Contains(lowerCaseTerm)
				|| e.Municipality.NameEn.ToLower().Contains(lowerCaseTerm)
                || e.Area.ToString().Contains(lowerCaseTerm));
			else
                municipalityLocation = municipalityLocation.Where(e => e.NameSq.ToLower().Contains(lowerCaseTerm)
                || e.Municipality.NameSq.ToLower().Contains(lowerCaseTerm)
                || e.Area.ToString().Contains(lowerCaseTerm));

            return municipalityLocation;
        }

		public static IQueryable<MunicipalityLocation> Sort(this IQueryable<MunicipalityLocation> municipalityLocation, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return municipalityLocation.OrderBy(e => e.NameSq);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(MunicipalityLocation).GetProperties(BindingFlags.Public |
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
				return municipalityLocation.OrderBy(e => e.NameSq);

			return municipalityLocation.OrderBy(orderQuery);
		}
	}

}
