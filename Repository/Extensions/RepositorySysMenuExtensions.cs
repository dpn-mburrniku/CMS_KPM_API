
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositorySysMenuExtensions
    {
        public static IQueryable<SysMenu> Search(this IQueryable<SysMenu> sysmenu, string query, int? webLangId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return sysmenu;

            var lowerCaseTerm = query.Trim().ToLower();

            if (webLangId == 2)
                sysmenu = sysmenu.Where(e => e.NameEn.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm)
                || e.Icon.ToLower().Contains(lowerCaseTerm)
                || e.Parent.NameEn.ToLower().Contains(lowerCaseTerm)
                );
            else if (webLangId == 3)
                sysmenu = sysmenu.Where(e => e.NameSr.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm)
                || e.Icon.ToLower().Contains(lowerCaseTerm)
                || e.Parent.NameSr.ToLower().Contains(lowerCaseTerm)
                );
            else
                sysmenu = sysmenu.Where(e => e.NameSq.ToLower().Contains(lowerCaseTerm)
                || e.Path.ToLower().Contains(lowerCaseTerm)
                || e.Icon.ToLower().Contains(lowerCaseTerm)
                || e.Parent.NameSq.ToLower().Contains(lowerCaseTerm)
                );
            return sysmenu;
        }

        public static IQueryable<SysMenu> Sort(this IQueryable<SysMenu> sysmenu, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return sysmenu.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(SysMenu).GetProperties(BindingFlags.Public |
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
                return sysmenu.OrderBy(e => e.OrderNo);

            return sysmenu.OrderBy(orderQuery);
        }
    }

}
