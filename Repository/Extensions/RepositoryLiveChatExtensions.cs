using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Repository.Extensions
{
    public static class RepositoryLiveChatExtensions
    {        
        public static IQueryable<LiveChat> FilterLiveChatByPage(this IQueryable<LiveChat> livechat, int? filter) =>
        livechat.Where(e => filter > 0 ? e.PageId == filter : true);
        public static IQueryable<LiveChat> FilterLiveChatByLanguage(this IQueryable<LiveChat> livechat, int? filter) =>
        livechat.Where(e => e.LanguageId == filter);

        public static IQueryable<LiveChat> Search(this IQueryable<LiveChat> livechat, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return livechat;

            var lowerCaseTerm = query.Trim().ToLower();

            livechat = livechat.Where(e => e.Description.ToLower().Contains(lowerCaseTerm) || e.Name.ToLower().Contains(lowerCaseTerm) || e.Page.PageName.ToLower().Contains(lowerCaseTerm));

            return livechat;
        }

        public static IQueryable<LiveChat> Sort(this IQueryable<LiveChat> livechat, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return livechat.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(LiveChat).GetProperties(BindingFlags.Public |
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
                return livechat.OrderBy(e => e.OrderNo);

            return livechat.OrderBy(orderQuery);
        }
    }
}
