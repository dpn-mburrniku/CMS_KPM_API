using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryEmailTemplateItemExtensions
    {
        public static IQueryable<EmailTemplateItem> FilterEmailTemplateItemByLanguage(this IQueryable<EmailTemplateItem> emailTemplateItem, int? filter) =>
        emailTemplateItem.Where(e => e.LanguageId == filter);

        public static IQueryable<EmailTemplateItem> FilterEmailTemplateItems(this IQueryable<EmailTemplateItem> emailTemplateItems, int? filter) =>
        emailTemplateItems.Where(e => filter > 0 ? e.EmailTemplateId == filter : true);

        public static IQueryable<EmailTemplateItem> Search(this IQueryable<EmailTemplateItem> emailTemplateItem, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return emailTemplateItem;

            var lowerCaseTerm = query.Trim().ToLower();

            emailTemplateItem = emailTemplateItem.Where(e => e.Name.ToLower().Contains(lowerCaseTerm));

            return emailTemplateItem;
        }

        public static IQueryable<EmailTemplateItem> Sort(this IQueryable<EmailTemplateItem> slide, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return slide.OrderBy(e => e.Id);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(EmailTemplateItem).GetProperties(BindingFlags.Public |
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
                return slide.OrderBy(e => e.Id);

            return slide.OrderBy(orderQuery);
        }
    }

}