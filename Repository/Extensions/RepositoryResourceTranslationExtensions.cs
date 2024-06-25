using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Entities.DataTransferObjects;

namespace Repository.Extensions
{
    public static class RepositoryResourceTranslationExtensions
    {
        public static IQueryable<ResourceTranslationString> FilterResourceTranslationByType(this IQueryable<ResourceTranslationString> translationType, int? filter) =>
        translationType.Where(e => filter > 0 ? e.TypeId == filter : true);

        public static IQueryable<ResourceTranslationString> FilterResourceTranslationByLanguage(this IQueryable<ResourceTranslationString> resource, int? filter) =>
        resource.Where(e => e.LanguageId == filter);

        public static IQueryable<ResourceTranslationString> Search(this IQueryable<ResourceTranslationString> resource, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return resource;

            var lowerCaseTerm = query.Trim().ToLower();

            resource = resource.Where(e => e.Name.ToLower().Contains(lowerCaseTerm) || e.Value.ToLower().Contains(lowerCaseTerm));

            return resource;
        }
        public static IQueryable<ResourceTranslationString> Sort(this IQueryable<ResourceTranslationString> model, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return model.OrderBy(e => e.Id);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(ResourceTranslationString).GetProperties(BindingFlags.Public |
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
                return model.OrderBy(e => e.Id);

            return model.OrderBy(orderQuery);
        }
    }
}
