using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<Contact?> GetContactById(int id, int webLangId)
        {
            var contact = await _cmsContext.Contacts.FindAsync(id, webLangId);

            return contact;
        }

        public async Task<PagedList<Contact>> GetContactAsync(ContactFilterParameters parameter)
        {
            IQueryable<Contact> data = _cmsContext.Contacts.Include(x => x.Layout).Include(x => x.Page).Include(x => x.Gender).IgnoreAutoIncludes().AsNoTracking()
                            .FilterContactByLanguage(parameter.webLangId)
                            .FilterContactByLayout(parameter.LayoutId)
                            .FilterContactByPage(parameter.PageId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Contact>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        #region web
        public async Task<List<ContactModel>> GetContact(int GjuhaId, int PageId)
        {
            var contactlLista = await (from k in _cmsContext.Contacts

                                       where k.LanguageId == GjuhaId && k.PageId == PageId
                                       select new ContactModel
                                       {
                                           Adresa = k.Address,
                                           Emaili = k.Email,
                                           GjuhaId = k.LanguageId,
                                           Gps = k.Latitude + "," + k.Longitude,
                                           Latitude = k.Latitude,
                                           Longitude = k.Longitude,
                                           KontaktiId = k.Id,
                                           KontaktiPershkrimi = k.Description,
                                           MediaId = k.MediaId,
                                           PageId = k.PageId,
                                           Telefoni = k.PhoneNumber,
                                           Telefoni2 = k.PhoneNumber2,
                                           Fax = k.Fax,
                                           PersoniKontaktues = k.ContactPerson,
                                           media = (from m in _cmsContext.Media
                                                    where m.Id == k.MediaId
                                                    select new MediaModel
                                                    {
                                                        MediaId = m.Id,
                                                        MediaEmri = m.FileName.ToString(),
                                                        MediaEmri_medium = m.FileNameMedium,
                                                        MediaEmri_small = m.FileNameSmall,
                                                        MediaEx = m.FileEx,
                                                        MediaPershkrimi = m.Name,
                                                        MediaDataInsertimit = m.Created,
                                                        MediaExKategoriaId = m.MediaExCategoryId,
                                                        IsOtherSource = m.IsOtherSource,
                                                        OtherSource = m.OtherSourceLink
                                                    }).FirstOrDefault()
                                       }).ToListAsync();

            return contactlLista;
        }

        public async Task<List<ContactModel>> GetContactList(int GjuhaId, int PageId)
        {
            var contactlLista = await (from k in _cmsContext.Contacts
                                       where k.LanguageId == GjuhaId && k.PageId == PageId
                                       && !string.IsNullOrEmpty(k.Email)
                                       select new ContactModel
                                       {
                                           KontaktiId = k.Id,
                                           Emaili = k.Email,
                                           KontaktiPershkrimi = k.Description + (string.IsNullOrEmpty(k.ContactPerson) ? "" : " (" + k.ContactPerson + ")")
                                       }).ToListAsync();

            return contactlLista;
        }
        #endregion
    }
}
