
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryFaqDetailExtensions
    { 
        public static IQueryable<Faqdetail> FilterFaqDetailByHeader(this IQueryable<Faqdetail> FaqDetail, int? filter) =>
        FaqDetail.Where(e => filter > 0 ? e.HeaderId == filter : true);
        public static IQueryable<Faqdetail> FilterFaqDetailByLanguage(this IQueryable<Faqdetail> FaqDetail, int? filter) =>
        FaqDetail.Where(e => e.LanguageId == filter);

        public static IQueryable<Faqdetail> Search(this IQueryable<Faqdetail> FaqDetail, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return FaqDetail;

            var lowerCaseTerm = query.Trim().ToLower();

            FaqDetail = FaqDetail.Where(e => e.Question.ToLower().Contains(lowerCaseTerm) || e.Answer.ToLower().Contains(lowerCaseTerm));

            return FaqDetail;
        }

        public static IQueryable<Faqdetail> Sort(this IQueryable<Faqdetail> FaqDetail, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return FaqDetail.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Faqdetail).GetProperties(BindingFlags.Public |
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
                return FaqDetail.OrderBy(e => e.OrderNo);

            return FaqDetail.OrderBy(orderQuery);
        }
    }

}
