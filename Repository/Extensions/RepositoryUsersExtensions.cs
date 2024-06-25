
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryUserExtensions
    {
        public static IQueryable<AspNetUser> FilterUsers(this IQueryable<AspNetUser> users, string filter) =>
        users.Where(e => (e.UserName.Contains(filter) || filter == null || filter == ""));

        public static IQueryable<AspNetUser> Search(this IQueryable<AspNetUser> users, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return users;

            var lowerCaseTerm = query.Trim().ToLower();

            users = users.Where(e => e.UserName.ToLower().Contains(lowerCaseTerm) || e.Email.ToLower().Contains(lowerCaseTerm) || e.Firstname.ToLower().Contains(lowerCaseTerm) || e.Lastname.ToLower().Contains(lowerCaseTerm));

            return users;
        }

        public static IQueryable<AspNetUser> Sort(this IQueryable<AspNetUser> users, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return users.OrderBy(e => e.UserName);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(AspNetUser).GetProperties(BindingFlags.Public |
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
                return users.OrderBy(e => e.UserName);

            return users.OrderBy(orderQuery);
        }
    }

}
