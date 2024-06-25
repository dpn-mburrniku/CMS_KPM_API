
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryPaymentExtensions
    {
        public static IQueryable<Payment> FilterPaymentByDate(this IQueryable<Payment> payment, DateTime? dateFrom, DateTime? dateTo) =>
        payment.Where(e => (dateFrom != null ? e.Date.Date >= dateFrom.Value : true) &&
                              (dateTo != null ? e.Date.Date <= dateTo.Value : true));

        public static IQueryable<Payment> FilterPaymentBySucceed(this IQueryable<Payment> payment, bool? filter) =>
        payment.Where(e => filter != null ? e.Succeed == filter : true);
        public static IQueryable<Payment> Search(this IQueryable<Payment> payment, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return payment;

            var lowerCaseTerm = query.Trim().ToLower();

            payment = payment.Where(e => e.Date.ToString().Contains(lowerCaseTerm) || e.Amount.ToString().Contains(lowerCaseTerm) || e.Succeed.ToString().Contains(lowerCaseTerm));

            return payment;
        }

        public static IQueryable<Payment> Sort(this IQueryable<Payment> payment, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return payment.OrderBy(e => e.Id);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Payment).GetProperties(BindingFlags.Public |
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
                return payment.OrderBy(e => e.Id);

            return payment.OrderBy(orderQuery);
        }
    }

}
