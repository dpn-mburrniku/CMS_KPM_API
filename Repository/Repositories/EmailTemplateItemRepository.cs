using Contracts.IServices;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

namespace Repository.Repositories
{
    internal class EmailTemplateItemRepository : GenericRepository<EmailTemplateItem>, IEmailTemplateItemRepository
    {
        public EmailTemplateItemRepository(CmsContext cmsContext) : base(cmsContext)
        { 
        }

        public async Task<EmailTemplateItem?> GetEmailTemplateItemById(int id, int webLangId)
        {

            var emailTemplateItem = await _cmsContext.EmailTemplateItems.FindAsync(id, webLangId);

            return emailTemplateItem;
        }
        public async Task<PagedList<EmailTemplateItem>> GetEmailTemplateItemAsync(EmailItemsFilterParameters parameter)
        {
            IQueryable<EmailTemplateItem> data = _cmsContext.EmailTemplateItems.Include(x => x.EmailTemplate).IgnoreAutoIncludes().AsNoTracking()
                                            .FilterEmailTemplateItemByLanguage(parameter.webLangId)
                                            .FilterEmailTemplateItems(parameter.EmailTemplateId)
                                            .Search(parameter.Query)
                                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);


            return PagedList<EmailTemplateItem>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }
    }
}