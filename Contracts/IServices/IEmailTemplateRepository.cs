using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
	public interface IEmailTemplateRepository : IGenericRepository<EmailTemplate>
	{
		Task<EmailTemplate?> GetEmailTemplateById(int id, int webLangId);
		Task<PagedList<EmailTemplate>> GetEmailTemplatesAsync(FilterParameters parameter);
	}
}
