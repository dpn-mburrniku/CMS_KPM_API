using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Entities.Models;

namespace Repository.Extensions
{
	public static class RepositoryPostExtensions
	{
		public static IQueryable<Post> FilterPostByDate(this IQueryable<Post> page, DateTime? dateFrom, DateTime? datoTo) =>
		page.Where(e => (dateFrom != null ?
												e.StartDate.Date >= dateFrom.Value &&
												(e.EndDate != null ? e.EndDate.Value.Date >= dateFrom.Value : true) : true)
										&& (datoTo != null ? e.StartDate.Date <= datoTo.Value : true));
		public static IQueryable<Post> FilterPostByLanguage(this IQueryable<Post> page, int? filter) =>
		page.Where(e => e.LanguageId == filter);
		public static IQueryable<Post> FilterPostByPostCategoryId(this IQueryable<Post> page, List<int>? postIds) =>
		page.Where(e => postIds == null ? true : postIds.Contains(e.Id));

		public static IQueryable<Post> FilterIsDeleted(this IQueryable<Post> page, bool isDeleted) =>
			page.Where(e => e.Deleted == isDeleted);

		public static IQueryable<Post> Search(this IQueryable<Post> page, string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				return page;

			var lowerCaseTerm = query.Trim().ToLower();

			page = page.Where(e => e.Title.ToLower().Contains(lowerCaseTerm) || e.Id.ToString().Contains(lowerCaseTerm));

			return page;
		}

		public static IQueryable<Post> Sort(this IQueryable<Post> page, string orderByQueryString)
		{
			if (string.IsNullOrWhiteSpace(orderByQueryString))
				return page.OrderByDescending(e => e.StartDate);

			var orderParams = orderByQueryString.Trim().Split(',');
			var propertyInfos = typeof(Post).GetProperties(BindingFlags.Public |
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
				return page.OrderByDescending(e => e.StartDate);

			return page.OrderBy(orderQuery);
		}
	}

}
