
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryLinkExtensions
    {
        public static IQueryable<Link> FilterLinkByLayout(this IQueryable<Link> link, int? filter) =>
        link.Where(e => e.LayoutId == filter);
        public static IQueryable<Link> FilterLinkByType(this IQueryable<Link> link, int? filter) =>
        link.Where(e => filter > 0 ? e.TypeId == filter : true);
        public static IQueryable<Link> FilterLinkByLanguage(this IQueryable<Link> link, int? filter) =>
        link.Where(e => e.LanguageId == filter);

        public static IQueryable<Link> Search(this IQueryable<Link> link, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return link;

            var lowerCaseTerm = query.Trim().ToLower();

            link = link.Where(e => e.LinkName.ToLower().Contains(lowerCaseTerm) || e.Url.ToLower().Contains(lowerCaseTerm) || e.LinkTarget.ToLower().Contains(lowerCaseTerm));

            return link;
        }

        public static IQueryable<Link> Sort(this IQueryable<Link> link, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return link.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Link).GetProperties(BindingFlags.Public |
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
                return link.OrderBy(e => e.OrderNo);

            return link.OrderBy(orderQuery);
        }
    }

}
