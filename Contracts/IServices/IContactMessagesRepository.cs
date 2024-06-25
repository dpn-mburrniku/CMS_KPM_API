using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;

namespace Contracts.IServices
{
    public interface IContactMessagesRepository : IGenericRepository<ContactMessage>
    {
        Task<ContactMessage?> GetContactMessageById(int id);
        Task<PagedList<ContactMessagesListDto>> GetContactMessageAsync(MailStringFilterParameters parameter);
        Task<int> GetUnReadMessages();

    }
}
