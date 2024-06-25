
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryPersonelExtensions
    {
        public static IQueryable<Personel> FilterPersonelByLayout(this IQueryable<Personel> personel, int? filter) =>
        personel.Where(e => e.LayoutId == filter);
        //personel.Where(e => filter > 0 ? e.LayoutId == filter : layoutIds.Contains(e.LayoutId));
        public static IQueryable<Personel> FilterPersonelByPage(this IQueryable<Personel> personel, int? filter) =>
        personel.Where(e => filter > 0 ? e.PageId == filter : true);
        public static IQueryable<Personel> FilterPersonelByLanguage(this IQueryable<Personel> personel, int? filter) =>
        personel.Where(e => e.LanguageId == filter);

        public static IQueryable<Personel> Search(this IQueryable<Personel> personel, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return personel;

            var lowerCaseTerm = query.Trim().ToLower();

            personel = personel.Where(e => e.Name.ToLower().Contains(lowerCaseTerm) || e.LastName.ToLower().Contains(lowerCaseTerm) || e.Position.ToLower().Contains(lowerCaseTerm) || e.Email.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm));

            return personel;
        }

        public static IQueryable<Personel> Sort(this IQueryable<Personel> contact, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return contact.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Personel).GetProperties(BindingFlags.Public |
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
                return contact.OrderBy(e => e.OrderNo);

            return contact.OrderBy(orderQuery);
        }
    }

}
