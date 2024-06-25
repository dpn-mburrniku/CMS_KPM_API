using Entities.Models; 
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects;

namespace Repository.Repositories
{
    public class ContactMessagesRepository : GenericRepository<ContactMessage>, IContactMessagesRepository
    {
		public ContactMessagesRepository(CmsContext cmsContext) : base(cmsContext)
		{			
		}
		public async Task<ContactMessage?> GetContactMessageById(int id)
		{
			var contactMessages = await _cmsContext.ContactMessages.FindAsync(id);

	
			return contactMessages;

		}
        public async Task<PagedList<ContactMessagesListDto>> GetContactMessageAsync(MailStringFilterParameters parameter)
        {

            //.ToListAsync();

            IQueryable<ContactMessagesListDto> query = from contactMessage in _cmsContext.ContactMessages.Search(parameter.Query).Sort(parameter.Sort.key + " " + parameter.Sort.order)
                                                       join layout in _cmsContext.Layouts on contactMessage.LayoutId equals layout.Id
                                                       //join page in _cmsContext.Pages on contactMessage.PageId equals page.Id
                                                       join contact in _cmsContext.Contacts.Where(x => x.LanguageId == parameter.webLangId) on contactMessage.ContactId equals contact.Id into groupContact
                                                       from c in groupContact.DefaultIfEmpty()
                                                       where contactMessage.LayoutId == parameter.LayoutId
                                                            && (parameter.PageId > 0 ? contactMessage.PageId == parameter.PageId : true)
                                                            && (parameter.ContactId > 0 ? contactMessage.ContactId == parameter.ContactId : true)
                                                            && (parameter.IsSent != null  ? contactMessage.IsSent == parameter.IsSent : true)
                                                            && contactMessage.IsDeleted == parameter.IsDeleted
                                                            && (parameter.IsDraft != null ? contactMessage.IsDraft == parameter.IsDraft : true)
                                                            && (parameter.IsRead != null ? contactMessage.IsRead == parameter.IsRead : true)
                                                            && (parameter.IsFavorite != null ? contactMessage.IsFavorite == parameter.IsFavorite : true)
                                                       select new ContactMessagesListDto()
                                                       {
                                                           EmailFrom = contactMessage.EmailFrom,
                                                           EmailTo = contactMessage.EmailTo,
                                                           ContactId = contactMessage.ContactId,
                                                           ContactName = c != null ? c.Description : "",
                                                           EmailCc = contactMessage.EmailCc,
                                                           EmailBcc = contactMessage.EmailBcc,
                                                           Id = contactMessage.Id,
                                                           LayoutId = contactMessage.LayoutId,
                                                           PageId = contactMessage.PageId,
                                                           Name = contactMessage.Name,
                                                           Subject = contactMessage.Subject,
                                                           Message = contactMessage.Message,
                                                           IsDraft = contactMessage.IsDraft,
                                                           IsFavorite  = contactMessage.IsFavorite,
                                                           IsSent = contactMessage.IsSent,
                                                           IsRead = contactMessage.IsRead,
                                                           IsDeleted = contactMessage.IsDeleted,
                                                           Created = contactMessage.Created,
                                                           CreatedBy = contactMessage.CreatedBy,
                                                       };

            //query = query.Distinct();

            return PagedList<ContactMessagesListDto>
                            .ToPagedList(query, parameter.PageIndex,
                            parameter.PageSize);
        }

        public async Task<int> GetUnReadMessages()
        {
            var contactMessages = await _cmsContext.ContactMessages.Where(x => x.IsRead == false).AsNoTracking().CountAsync();

            return contactMessages;
        }

        //public Task<PagedList<ContactMessage>> GetContactMessageAsync(FilterParameters parameter)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
