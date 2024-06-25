
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryLinkTypesExtensions
    {
       

        public static IQueryable<LinkType> FilterLinkTypesByLanguage(this IQueryable<LinkType> link, int? filter) =>
        link.Where(e => e.LanguageId == filter);

        public static IQueryable<LinkType> FilterLinkTypesByLocation(this IQueryable<LinkType> link, int? filter) =>
        link.Where(e => filter > 0 ? e.ComponentLocationId == filter : true);

        public static IQueryable<LinkType> Search(this IQueryable<LinkType> link, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return link;

            var lowerCaseTerm = query.Trim().ToLower();

            link = link.Where(e => e.LinkuTypeName.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleSq.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleEn.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleSr.ToLower().Contains(lowerCaseTerm));

            return link;
        }

        public static IQueryable<LinkType> Sort(this IQueryable<LinkType> link, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return link.OrderBy(e => e.LinkTypeId);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(LinkType).GetProperties(BindingFlags.Public |
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
                return link.OrderBy(e => e.LinkTypeId);

            return link.OrderBy(orderQuery);
        }
    }

}
