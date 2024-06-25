using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<Contact?> GetContactById(int id, int webLangId);
        //Task<PagedList<Contact>> GetContactAsync(ContactFilterParameters parameter, List<int> layoutIds);


        // web 
        Task<List<ContactModel>> GetContact(int GjuhaId, int LinkTypeID);
        Task<List<ContactModel>> GetContactList(int GjuhaId, int LinkTypeID);
        Task<PagedList<Contact>> GetContactAsync(ContactFilterParameters parameter);
    }
}
