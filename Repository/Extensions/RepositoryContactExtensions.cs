
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryContactExtensions
    {
        public static IQueryable<Contact> FilterContactByLayout(this IQueryable<Contact> contact, int? filter) =>
        contact.Where(e => e.LayoutId == filter);
	  	  public static IQueryable<Contact> FilterContactByPage(this IQueryable<Contact> contact, int? filter) =>
        contact.Where(e => filter > 0 ? e.PageId == filter : true);

        public static IQueryable<Contact> FilterContactByLanguage(this IQueryable<Contact> contact, int? filter) =>
        contact.Where(e => e.LanguageId == filter);

        public static IQueryable<Contact> Search(this IQueryable<Contact> contact, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return contact;

            var lowerCaseTerm = query.Trim().ToLower();

            contact = contact.Where(e => e.Description.ToLower().Contains(lowerCaseTerm) || e.ContactPerson.ToLower().Contains(lowerCaseTerm) || e.Gender.Name.ToLower().Contains(lowerCaseTerm) || e.Email.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm));

            return contact;
        }

        public static IQueryable<Contact> Sort(this IQueryable<Contact> contact, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return contact.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Contact).GetProperties(BindingFlags.Public |
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
