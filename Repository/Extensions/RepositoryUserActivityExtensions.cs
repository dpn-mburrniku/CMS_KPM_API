using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;

namespace Repository.Extensions
{
    public static class RepositoryUserActivityExtensions
    {
        public static IQueryable<UserAudit> FilterUserActivityByDate(this IQueryable<UserAudit> log, DateTime? dateFrom, DateTime? datoTo) =>
        log.Where(e => (dateFrom.HasValue ? e.Date.Date >= dateFrom.Value : true)
                        && (datoTo.HasValue ? e.Date.Date <= datoTo.Value : true));
        public static IQueryable<UserAudit> FilterUserActivityByUserId(this IQueryable<UserAudit> log, string? filter) =>
        log.Where(e => string.IsNullOrEmpty(filter) ? true : e.UserId == filter);
        public static IQueryable<UserAudit> FilterUserActivityByActionTypeId(this IQueryable<UserAudit> log, int? filter) =>
        log.Where(e => filter != null ? e.ActionType == filter :true);

        public static IQueryable<UserAudit> Sort(this IQueryable<UserAudit> log, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return log.OrderByDescending(e => e.Date);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(UserAudit).GetProperties(BindingFlags.Public |
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
                return log.OrderByDescending(e => e.Date);

            return log.OrderBy(orderQuery);
        }
    }
}