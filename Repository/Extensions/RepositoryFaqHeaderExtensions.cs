
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryFaqHeaderExtensions
    {
        public static IQueryable<Faqheader> FilterFaqHeaderByLayout(this IQueryable<Faqheader> FaqHeader, int? filter) =>
        //FaqHeader.Where(e => filter > 0 ? e.LayoutId == filter : layoutIds.Contains(e.LayoutId));
        FaqHeader.Where(e => e.LayoutId == filter);
        public static IQueryable<Faqheader> FilterFaqHeaderByPage(this IQueryable<Faqheader> FaqHeader, int? filter) =>
        FaqHeader.Where(e => filter > 0 ? e.PageId == filter : true);
        public static IQueryable<Faqheader> FilterFaqHeaderByLanguage(this IQueryable<Faqheader> FaqHeader, int? filter) =>
        FaqHeader.Where(e => e.LanguageId == filter);

        public static IQueryable<Faqheader> Search(this IQueryable<Faqheader> FaqHeader, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return FaqHeader;

            var lowerCaseTerm = query.Trim().ToLower();

            FaqHeader = FaqHeader.Where(e => e.Title.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm));

            return FaqHeader;
        }

        public static IQueryable<Faqheader> Sort(this IQueryable<Faqheader> FaqHeader, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return FaqHeader.OrderBy(e => e.Title);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Faqheader).GetProperties(BindingFlags.Public |
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
                return FaqHeader.OrderBy(e => e.Title);

            return FaqHeader.OrderBy(orderQuery);
        }
    }

}
