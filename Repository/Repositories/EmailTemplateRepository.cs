using Contracts.IServices;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
	public class EmailTemplateRepository : GenericRepository<EmailTemplate>, IEmailTemplateRepository
	{
		public EmailTemplateRepository(CmsContext cmsContext) : base(cmsContext)
		{
		}

		public async Task<EmailTemplate?> GetEmailTemplateById(int id, int webLangId)
		{

			var emailTemplate = await _cmsContext.EmailTemplates.FindAsync(id, webLangId);

			return emailTemplate;
		}
		public async Task<PagedList<EmailTemplate>> GetEmailTemplatesAsync(FilterParameters parameter)
		{
			IQueryable<EmailTemplate> data = _cmsContext.EmailTemplates.AsNoTracking()
											.FilterEmailTemplateByLanguage(parameter.webLangId)
											.Search(parameter.Query)
											.Sort(parameter.Sort.key + " " + parameter.Sort.order);


			return PagedList<EmailTemplate>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}
	}
}
