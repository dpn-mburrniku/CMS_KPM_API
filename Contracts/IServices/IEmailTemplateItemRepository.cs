using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IEmailTemplateItemRepository : IGenericRepository<EmailTemplateItem>
    {
        Task<EmailTemplateItem?> GetEmailTemplateItemById(int id, int webLangId);
        Task<PagedList<EmailTemplateItem>> GetEmailTemplateItemAsync(EmailItemsFilterParameters parameter);
    }
}
