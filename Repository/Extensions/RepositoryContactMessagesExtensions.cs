using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models; using CMS.API;

namespace Repository.Extensions
{
	public static class RepositoryContactMessagesExtensions
	{
		public static IQueryable<ContactMessage> Search(this IQueryable<ContactMessage> contactMessage, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return contactMessage;

			var lowerCaseTerm = query.Trim().ToLower();

			contactMessage = contactMessage.Where(e => e.Message.ToLower().Contains(lowerCaseTerm) || e.Name.ToLower().Contains(lowerCaseTerm) || e.EmailFrom.ToLower().Contains(lowerCaseTerm) || e.Subject.ToLower().Contains(lowerCaseTerm));

			return contactMessage;
		}

		public static IQueryable<ContactMessage> Sort(this IQueryable<ContactMessage> contactMessage, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return contactMessage.OrderByDescending(e => e.Id);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(ContactMessage).GetProperties(BindingFlags.Public |
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
				return contactMessage.OrderByDescending(e => e.Id);

			return contactMessage.OrderBy(orderQuery);
		}
	}

}
