﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;
using CMS.API;

namespace Repository.Extensions
{
    public static class RepositoryLanguageExtensions
    {
        public static IQueryable<Language> FilterLanguages(this IQueryable<Language> language, int? filter) =>
        language.Where(e => filter > 0 ? e.CultureCodeId == filter : true);

        public static IQueryable<Language> Search(this IQueryable<Language> model, string query, int? webLangId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return model;

            var lowerCaseTerm = query.Trim().ToLower();

            if (webLangId == 2)
                model = model.Where(e => e.NameEn.ToLower().Contains(lowerCaseTerm)
                );
            else if (webLangId == 3)
                model = model.Where(e => e.NameSr.ToLower().Contains(lowerCaseTerm));
            else
                model = model.Where(e => e.NameSq.ToLower().Contains(lowerCaseTerm));

            return model;
        }

        public static IQueryable<Language> Sort(this IQueryable<Language> model, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return model.OrderBy(e => e.Id);

            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Language).GetProperties(BindingFlags.Public |
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
                return model.OrderBy(e => e.Id);

            return model.OrderBy(orderQuery);
        }

    }
}
