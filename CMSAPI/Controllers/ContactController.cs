using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Entities.Models;
using NetTopologySuite.Index.HPRtree;
using Abp.Linq.Expressions;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public ContactController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [Authorize]
        [HttpGet]
        [Route("GetContacts")]
        public async Task<IActionResult> GetContacts(int webLangId = 1)
        {
            string[] includes = { "Layout", "Page", "Gender" };
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            List<int> layoutIds = layouts.Select(x => x.Id).ToList();

            var contactQuery = PredicateBuilder.False<Contact>();

            foreach (var id in layoutIds)
            {
                contactQuery = contactQuery.Or(t => t.LayoutId == id);
            }
            contactQuery = contactQuery.And(t => t.LanguageId == webLangId);

            var ContactsList = await _unitOfWork.Contacts.FindByCondition(contactQuery, false, includes).ToListAsync();

            if (ContactsList.Count > 0)
            {
                var contactsDto = _mapper.Map<IEnumerable<ContactListDto>?>(ContactsList);
                return Ok(contactsDto);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        [Route("GetContactsByPage")]
        public async Task<IActionResult> GetContactsByPage(int pageId, int webLangId = 1)
        {
            var contactIds = await _unitOfWork.Contacts.FindByCondition(t => (pageId > 0 ? t.PageId == pageId : true) && t.LanguageId == webLangId, false, null).ToListAsync();
            
            var contactsDto = _mapper.Map<IEnumerable<ContactDto>?>(contactIds);

            return Ok(contactsDto);
        }

        [Authorize]
        [HttpPost]
        [Route("GetContactsAsync")]
        public async Task<IActionResult> GetContactsAsync([FromBody] ContactFilterParameters parameter)
        {
            //var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            //var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            //var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
            //List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var contact = await _unitOfWork.Contacts.GetContactAsync(parameter);
            var contactsDto = _mapper.Map<IEnumerable<ContactListDto>?>(contact);

            return Ok(new
            {
                data = contactsDto,
                total = contact.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpPost]
        [Route("CreateContact")]
        public async Task<IActionResult> CreateContact([FromBody] ContactDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int ContactId = _unitOfWork.Contacts.GetMaxPK(i => i.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = ContactId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var ContactList = await _unitOfWork.Contacts.FindByCondition(t => t.LanguageId == item.Id
                                                                                  && t.LayoutId == model.LayoutId && t.PageId == model.PageId, false, null).ToListAsync();
                        if (ContactList.Where(t => t.OrderNo == 1).Count() > 0)
                        {
                            var contactToReturn = _mapper.Map<List<UpdateContactListDto>>(ContactList);
                            await UpdateContactsOrderBack(contactToReturn);
                        }
                        var multiContactEntity = _mapper.Map<Contact>(model);
                        multiContactEntity.LanguageId = item.Id;
                        multiContactEntity.CreatedBy = userinId;
                        multiContactEntity.Created = DateTime.Now;
                        multiContactEntity.OrderNo = 1;
                        await _unitOfWork.Contacts.Create(multiContactEntity);

                        await _unitOfWork.Contacts.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created, new { Id = ContactId });
                }

                var ContactListOneLang = await _unitOfWork.Contacts.FindByCondition(t => t.LanguageId == model.LanguageId
                                                                           && t.LayoutId == model.LayoutId && t.PageId == model.PageId, false, null).ToListAsync();
                if (ContactListOneLang.Where(t => t.OrderNo == 1).Count() > 0)
                {
                    var contactToReturnOneLang = _mapper.Map<List<UpdateContactListDto>>(ContactListOneLang);
                    await UpdateContactsOrderBack(contactToReturnOneLang);
                }
                var ContactEntity = _mapper.Map<Contact>(model);
                ContactEntity.CreatedBy = userinId;
                ContactEntity.Created = DateTime.Now;
                ContactEntity.OrderNo = 1;
                await _unitOfWork.Contacts.Create(ContactEntity);

                await _unitOfWork.Contacts.Commit();

                return StatusCode(StatusCodes.Status201Created, new { Id = ContactId });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpGet("GetContactById")]
        public async Task<IActionResult> GetContactById(int id, int webLangId)
        {
            var data = await _unitOfWork.Contacts.GetContactById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<ContactDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPut("UpdateContact")]
        public async Task<IActionResult> UpdateContact([FromBody] ContactDto model)
        {
            var contactEntity = await _unitOfWork.Contacts.GetContactById(model.Id, model.LanguageId);

            if (contactEntity == null)
            {
                int? pageId = null;

                if (model.PageId != null)
                {
                    var page = await _unitOfWork.Pages.GetPageById((int)model.PageId, model.LanguageId);
                    if(page != null)
                        pageId = page.Id;
                }
                contactEntity = _mapper.Map<Contact>(model);
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                contactEntity.PageId = pageId;
                contactEntity.CreatedBy = userinId;
                contactEntity.Created = DateTime.Now;
                await _unitOfWork.Contacts.Create(contactEntity);
            }
            else
            {
                _mapper.Map(model, contactEntity);
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                contactEntity.ModifiedBy = userinId;
                contactEntity.Modified = DateTime.Now;

                _unitOfWork.Contacts.Update(contactEntity);
            }

         
            await _unitOfWork.Contacts.Commit();

            var menuToReturn = _mapper.Map<ContactDto>(contactEntity);
            return Ok(menuToReturn);

        }

        [Authorize]
        [HttpDelete("DeleteContact")]
        public async Task<IActionResult> DeleteContact(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var contactId = await _unitOfWork.Contacts.GetContactById(id, webLangId);

            if (contactId == null)
            {
                return NotFound();
            }

            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multiContactEntity = await _unitOfWork.Contacts.GetContactById(id, item.Id);
                    if (multiContactEntity != null)
                    {
                        _unitOfWork.Contacts.Delete(multiContactEntity);
                        await _unitOfWork.Contacts.Commit();
                    }
                }

                return Ok();
            }

            _unitOfWork.Contacts.Delete(contactId);

            await _unitOfWork.Contacts.Commit();

            return Ok();
        }


        [Authorize]
        [HttpPost]
        [Route("OrderContact")]
        public async Task<ActionResult> OrderContact(List<UpdateContactListDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var contact = await _unitOfWork.Contacts.GetContactById(item.Id, item.LanguageId);
                    if (contact != null)
                    {
                        contact.OrderNo = item.OrderNo;
                        _unitOfWork.Contacts.Update(contact);
                        await _unitOfWork.Contacts.Commit();
                    }
                }
                return Ok();
            }

            return NotFound();
        }

        private async Task<bool> UpdateContactsOrderBack(List<UpdateContactListDto>? model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var contact = await _unitOfWork.Contacts.GetContactById(item.Id, item.LanguageId);
                    if (contact != null)
                    {
                        contact.OrderNo = item.OrderNo + 1;
                        _unitOfWork.Contacts.Update(contact);
                    }
                }
                await _unitOfWork.Contacts.Commit();
                return true;
            }
            return false;
        }
    }
}
