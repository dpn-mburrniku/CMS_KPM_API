
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using Entities.Models; using CMS.API;
using Entities.Models;

namespace Repository.Extensions
{
    public static class RepositorySocialNetworkExtensions
    {
        public static IQueryable<SocialNetwork> FilterSocialNetworkByLayout(this IQueryable<SocialNetwork> socialNetwork, int? filter) =>
        //socialNetwork.Where(e => filter > 0 ? e.LayoutId == filter : layoutIds.Contains(e.LayoutId));
        socialNetwork.Where(e => e.LayoutId == filter);
        public static IQueryable<SocialNetwork> FilterSocialNetworkByLocation(this IQueryable<SocialNetwork> socialNetwork, int? filter) =>
        socialNetwork.Where(e => filter > 0 ? e.ComponentLocationId == filter : true);
        public static IQueryable<SocialNetwork> FilterSocialNetworkByLanguage(this IQueryable<SocialNetwork> socialNetwork, int? filter) =>
        socialNetwork.Where(e => e.LanguageId == filter);
        public static IQueryable<SocialNetwork> FilterSocialNetwork(this IQueryable<SocialNetwork> socialNetwork, string filter) =>
        socialNetwork.Where(e => (e.Name.Contains(filter) || filter == null || filter == ""));

        public static IQueryable<SocialNetwork> Search(this IQueryable<SocialNetwork> socialNetwork, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return socialNetwork;

            var lowerCaseTerm = query.Trim().ToLower();

            socialNetwork = socialNetwork.Where(e => e.Name.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleSq.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleEn.ToLower().Contains(lowerCaseTerm) || e.ComponentLocation.TitleSr.ToLower().Contains(lowerCaseTerm)
            || e.Layout.NameSq.ToLower().Contains(lowerCaseTerm) || e.Layout.NameEn.ToLower().Contains(lowerCaseTerm) || e.Layout.NameSr.ToLower().Contains(lowerCaseTerm)
            || e.Link.ToLower().Contains(lowerCaseTerm) || e.ImgPath.ToLower().Contains(lowerCaseTerm) || e.Html.ToLower().Contains(lowerCaseTerm));

            return socialNetwork;
        }

        public static IQueryable<SocialNetwork> Sort(this IQueryable<SocialNetwork> socialNetwork, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return socialNetwork.OrderBy(e => e.OrderNo);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(SocialNetwork).GetProperties(BindingFlags.Public |
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
                return socialNetwork.OrderBy(e => e.OrderNo);

            return socialNetwork.OrderBy(orderQuery);
        }
    }

}
