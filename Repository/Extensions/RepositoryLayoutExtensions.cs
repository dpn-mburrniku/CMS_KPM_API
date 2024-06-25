
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;
using System.Data;

namespace Repository.Extensions
{
    public static class RepositoryLayoutExtensions
    {
        //public static IQueryable<Layout> FilterLayout(this IQueryable<Layout> layout, string filter) =>
        //layout.Where(e => (e.NameSq.Contains(filter) || filter == null || filter == ""));

        public static IQueryable<Layout> Search(this IQueryable<Layout> layout, string query, int? webLangId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return layout;

            var lowerCaseTerm = query.Trim().ToLower();

            if (webLangId == 2)
                layout = layout.Where(e => e.NameEn.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm)
                );
            else if (webLangId == 3)
                layout = layout.Where(e => e.NameSr.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm));
            else
                layout = layout.Where(e => e.NameSq.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm));

            return layout;
        }

        public static IQueryable<Layout> Sort(this IQueryable<Layout> layout, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return layout.OrderBy(e => e.NameSq);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Layout).GetProperties(BindingFlags.Public |
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
                return layout.OrderBy(e => e.NameSq);

            return layout.OrderBy(orderQuery);
        }
    }

}
