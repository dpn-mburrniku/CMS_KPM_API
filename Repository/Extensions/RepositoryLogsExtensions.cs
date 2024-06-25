using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;

namespace Repository.Extensions
{
    public static class RepositoryLogsExtensions
    {
        public static IQueryable<Log> FilterLogsByDate(this IQueryable<Log> log, DateTime? dateFrom, DateTime? datoTo) =>
            log.Where(e => (dateFrom.HasValue ? e.InsertedDate.Date >= dateFrom.Value : true)
                            && (datoTo.HasValue ? e.InsertedDate.Date <= datoTo.Value : true));
        public static IQueryable<Log> FilterLogsByUsername(this IQueryable<Log> log, string? filter) =>
        log.Where(e => string.IsNullOrEmpty(filter) ? true : e.UserName == filter);

        public static IQueryable<Log> FilterLogsByMethod(this IQueryable<Log> log, string? filter) =>
        log.Where(e => string.IsNullOrEmpty(filter) ? true : e.HttpMethod == filter);

        public static IQueryable<Log> FilterLogsByController(this IQueryable<Log> log, string? filter) =>
        log.Where(e => string.IsNullOrEmpty(filter) ? true : e.Controller == filter);

        public static IQueryable<Log> FilterIsError(this IQueryable<Log> log, bool isDeleted) =>
        log.Where(e => e.IsError == isDeleted);

        public static IQueryable<Log> Search(this IQueryable<Log> log, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return log;

            var lowerCaseTerm = query.Trim().ToLower();

            log = log.Where(e => e.Action.ToLower().Contains(lowerCaseTerm) || e.Id.ToString().Contains(lowerCaseTerm));

            return log;
        }

        public static IQueryable<Log> Sort(this IQueryable<Log> log, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return log.OrderByDescending(e => e.InsertedDate);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Log).GetProperties(BindingFlags.Public |
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
                return log.OrderByDescending(e => e.InsertedDate);

            return log.OrderBy(orderQuery);
        }
    }

}
