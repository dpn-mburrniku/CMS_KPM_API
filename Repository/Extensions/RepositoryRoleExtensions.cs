
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryRoleExtensions
    {
        public static IQueryable<AspNetRole> FilterRole(this IQueryable<AspNetRole> role, string filter) =>
        role.Where(e => (e.Name.Contains(filter) || filter == null || filter == ""));

        public static IQueryable<AspNetRole> Search(this IQueryable<AspNetRole> role, string query, int? webLangId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return role;

            var lowerCaseTerm = query.Trim().ToLower();

            if (webLangId == 2)
                role = role.Where(e => e.NameEn.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm)
                );
            else if (webLangId == 3)
                role = role.Where(e => e.NameSr.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm));
            else
                role = role.Where(e => e.NameSq.ToLower().Contains(lowerCaseTerm)
                || e.Description.ToLower().Contains(lowerCaseTerm));

            return role;
        }

        public static IQueryable<AspNetRole> Sort(this IQueryable<AspNetRole> role, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return role.OrderBy(e => e.Name);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(AspNetRole).GetProperties(BindingFlags.Public |
                        BindingFlags.Instance);

            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0] == "label" ? "name" : param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>

                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            if (string.IsNullOrWhiteSpace(orderQuery))
                return role.OrderBy(e => e.Name);

            return role.OrderBy(orderQuery);
        }
    }

}
