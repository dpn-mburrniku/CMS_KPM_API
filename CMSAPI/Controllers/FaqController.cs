using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using NetTopologySuite.Index.HPRtree;
using Entities.DataTransferObjects.WebDTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FaqController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public IMapper _mapper { get; }

		public FaqController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
            _configuration = configuration;
        }

		#region ContactMessage
		[Authorize]
		[HttpPost]
		[Route("GetContactMessagesAsync")]
		public async Task<IActionResult> GetContactMessagesAsync([FromBody] MailStringFilterParameters parameter)
		{
			var ContactMessage = await _unitOfWork.ContactMessages.GetContactMessageAsync(parameter);
			var contactsDto = _mapper.Map<IEnumerable<ContactMessagesListDto>?>(ContactMessage);

			return Ok(new
			{
				data = contactsDto,
				total = ContactMessage.MetaData.TotalCount
			});
		}

        [Authorize]
        [HttpGet]
        [Route("GetContactMessages")]
        public async Task<IActionResult> GetUnReadMessages()
        {

            var ContactMessages = await _unitOfWork.ContactMessages.GetUnReadMessages();

            return Ok(ContactMessages);
        }


        [Authorize]
		[HttpGet("GetContactMessageById")]
		public async Task<IActionResult> GetContactMessageById(int id)
		{
			var data = await _unitOfWork.ContactMessages.GetContactMessageById(id);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				if (!data.IsRead)
				{
					data.IsRead = true;
					_unitOfWork.ContactMessages.Update(data);
					await _unitOfWork.ContactMessages.Commit();
				}
				var dataDto = _mapper.Map<ContactMessagesListDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateContactMessage")]
		public async Task<IActionResult> CreateContactMessage([FromBody] ContactMessagesDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				var ContactMessageEntity = _mapper.Map<ContactMessage>(model);
				ContactMessageEntity.IsRead = true;
				ContactMessageEntity.CreatedBy = userinId;
				ContactMessageEntity.Created = DateTime.Now;
				await _unitOfWork.ContactMessages.Create(ContactMessageEntity);


				bool succes = await SendMailPost(model.EmailFrom.ToLower(), model.EmailTo.ToLower(), model.EmailCc.ToLower(), model.EmailBcc.ToLower(), model.Subject, model.Message);
				if (succes)
				{
					await _unitOfWork.ContactMessages.Commit();
					return StatusCode(StatusCodes.Status201Created, ContactMessageEntity);
				}
			}
			return BadRequest(ModelState);
		}

		[Authorize]
		[HttpPut("UpdateContactMessage")]
		public async Task<IActionResult> UpdateContactMessage([FromBody] UpdateContactMessagesDto model)
		{
			var contactMessageEntity = await _unitOfWork.ContactMessages.GetContactMessageById(model.Id);

			if (contactMessageEntity == null)
			{
				return NotFound();
			}
			contactMessageEntity.IsRead = true;
			_mapper.Map(model, contactMessageEntity);

            _unitOfWork.ContactMessages.Update(contactMessageEntity);
			await _unitOfWork.ContactMessages.Commit();

			var menuToReturn = _mapper.Map<ContactMessagesDto>(contactMessageEntity);
			return Ok(menuToReturn);
        }


		[Authorize]
		[HttpDelete("DeleteContactMessage")]
		public async Task<IActionResult> DeleteContactMessage(int id)
		{
			var contactMessageId = await _unitOfWork.ContactMessages.GetContactMessageById(id);

			if (contactMessageId == null)
			{
				return NotFound();
			}

			_unitOfWork.ContactMessages.Delete(contactMessageId);

			await _unitOfWork.ContactMessages.Commit();

			return Ok();
		}


        [HttpGet]
        public async Task<bool> SendMailPost(string emailFrom, string emailTo,  string emailCc, string emailBcc, string subject, string body)
        {
            try
            {
				var _emailTo = emailTo.Split(';');
				var _emailCc = emailCc.Split(";");
				var _emailBcc = emailBcc.Split(";");
                MailMessage msg = new();
                msg.From = new MailAddress(emailFrom);
				foreach (var item in _emailTo)
				{
					msg.To.Add(new MailAddress(item));
				}
				foreach (var item in _emailCc)
				{
					if(!string.IsNullOrEmpty(item))
						msg.CC.Add(new MailAddress(item));
                }
                foreach (var item in _emailBcc)
                {
					if(!string.IsNullOrEmpty(item))
						msg.Bcc.Add(new MailAddress(item));
                }
                msg.Subject = subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new(_configuration.GetValue<string>("EmailSettings:Smtp"), Convert.ToInt32(_configuration.GetValue<int>("EmailSettings:Port")));
                System.Net.NetworkCredential credential = new(_configuration.GetValue<string>("EmailSettings:Email"), _configuration.GetValue<string>("EmailSettings:Password"));
                smtpClient.Credentials = credential;
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(msg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region FaqHeader
        [Authorize]
		[HttpGet]
		[Route("GetFaqHeaders")]
		public async Task<IActionResult> GetFaqHeaders()
		{
			string[] includes = { "Layout", "Page" };
			var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var FaqHeaderList = await _unitOfWork.FaqHeaders.FindByCondition(t => t.LanguageId == _unitOfWork.BaseConfig.GetCurrentUserLanguage()
			&& layoutIds.Contains(t.LayoutId), false, includes).ToListAsync();


			if (FaqHeaderList.Count > 0)
			{
				var FaqHeadersDto = _mapper.Map<IEnumerable<FaqHeaderListDto>?>(FaqHeaderList);
				return Ok(FaqHeadersDto);
			}

			return NotFound();
		}

		[Authorize]
		[HttpPost]
		[Route("GetFaqHeadersAsync")]
		public async Task<IActionResult> GetFaqHeadersAsync([FromBody] FaqHeaderParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			//List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var FaqHeader = await _unitOfWork.FaqHeaders.GetFaqHeaderAsync(parameter);
			var FaqHeadersDto = _mapper.Map<IEnumerable<FaqHeaderListDto>?>(FaqHeader);

			return Ok(new
			{
				data = FaqHeadersDto,
				total = FaqHeader.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetFaqHeaderById")]
		public async Task<IActionResult> GetFaqHeaderById(int id, int webLangId)
		{
			var data = await _unitOfWork.FaqHeaders.GetFaqHeaderById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<FaqHeaderDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateFaqHeader")]
		public async Task<IActionResult> CreateFaqHeader([FromBody] FaqHeaderDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int FaqHeaderId = _unitOfWork.FaqHeaders.GetMaxPK(a => a.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = FaqHeaderId;
				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multiFaqHeaderEntity = _mapper.Map<Faqheader>(model);
						multiFaqHeaderEntity.LanguageId = item.Id;
						multiFaqHeaderEntity.CreatedBy = userinId;
						multiFaqHeaderEntity.Created = DateTime.Now;
						await _unitOfWork.FaqHeaders.Create(multiFaqHeaderEntity);
						await _unitOfWork.FaqHeaders.Commit();
					}

					return StatusCode(StatusCodes.Status201Created, new { Id = FaqHeaderId });
				}

				var FaqHeaderEntity = _mapper.Map<Faqheader>(model);
				FaqHeaderEntity.CreatedBy = userinId;
				FaqHeaderEntity.Created = DateTime.Now;
				await _unitOfWork.FaqHeaders.Create(FaqHeaderEntity);

				await _unitOfWork.FaqHeaders.Commit();

				return StatusCode(StatusCodes.Status201Created, new { Id = FaqHeaderId });
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateFaqHeader")]
		public async Task<IActionResult> UpdateFaqHeader([FromBody] FaqHeaderDto model)
		{
			var FaqHeaderEntity = await _unitOfWork.FaqHeaders.GetFaqHeaderById(model.Id, model.LanguageId);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            if (FaqHeaderEntity == null)
			{
                int? pageId = null;

                if (model.PageId != null)
                {
                    var page = await _unitOfWork.Pages.GetPageById((int)model.PageId, model.LanguageId);
                    if (page != null)
                        pageId = page.Id;
                }
                FaqHeaderEntity = _mapper.Map<Faqheader>(model);
                FaqHeaderEntity.PageId = pageId;
                FaqHeaderEntity.CreatedBy = userinId;
                FaqHeaderEntity.Created = DateTime.Now;
                await _unitOfWork.FaqHeaders.Create(FaqHeaderEntity);
			}
			else
			{
                _mapper.Map(model, FaqHeaderEntity);
                FaqHeaderEntity.ModifiedBy = userinId;
                FaqHeaderEntity.Modified = DateTime.Now;
                _unitOfWork.FaqHeaders.Update(FaqHeaderEntity);
            }

			await _unitOfWork.FaqHeaders.Commit();

			var FaqHeaderToReturn = _mapper.Map<FaqHeaderDto>(FaqHeaderEntity);

			return Ok(FaqHeaderToReturn);
		}

		[Authorize]
		[HttpDelete("DeleteFaqHeader")]
		public async Task<IActionResult> DeleteFaqHeader(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var FaqHeader = await _unitOfWork.FaqHeaders.GetFaqHeaderById(id, webLangId);

			if (FaqHeader == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiFaqHeaderEntity = await _unitOfWork.FaqHeaders.GetFaqHeaderById(id, item.Id);
					if (multiFaqHeaderEntity != null)
					{
						_unitOfWork.FaqHeaders.Delete(multiFaqHeaderEntity);
						await _unitOfWork.FaqHeaders.Commit();
					}
				}
			}
			else
			{
				_unitOfWork.FaqHeaders.Delete(FaqHeader);

				await _unitOfWork.FaqHeaders.Commit();
			}
			return Ok();
		}
		#endregion

		#region Faqdetail
		[Authorize]
		[HttpGet]
		[Route("GetFaqDetails")]
		public async Task<IActionResult> GetFaqDetails(int? faqHeaderId, int webLangId = 1)
		{
			string[] includes = { "Faqheader" };
			var FaqDetailList = await _unitOfWork.FaqDetails.FindAll(false, includes).Where(t => t.LanguageId == webLangId && t.HeaderId == faqHeaderId).OrderBy(x=>x.OrderNo).ToListAsync();
			var faqdetailsDto = _mapper.Map<IEnumerable<FaqDetailListDto>?>(FaqDetailList);
			return Ok(faqdetailsDto);
		}

		[Authorize]
		[HttpPost]
		[Route("GetFaqDetailsAsync")]
		public async Task<IActionResult> GetFaqDetailsAsync([FromBody] FaqDetailParameters parameter)
		{
			var faqdetail = await _unitOfWork.FaqDetails.GetFaqDetailAsync(parameter);
			var faqdetailsDto = _mapper.Map<IEnumerable<FaqDetailListDto>?>(faqdetail);

			return Ok(new
			{
				data = faqdetailsDto,
				total = faqdetail.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetFaqDetailById")]
		public async Task<IActionResult> GetFaqDetailById(int id, int webLangId)
		{
			var data = await _unitOfWork.FaqDetails.GetFaqDetailById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<FaqDetailDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateFaqDetail")]
		public async Task<IActionResult> CreateFaqDetail([FromBody] FaqDetailDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int faqdetailId = _unitOfWork.FaqDetails.GetMaxPK(i => i.Id);
				var langList = _unitOfWork.FaqHeaders.FindByCondition(x => x.Id == model.HeaderId, false, null);

				model.Id = faqdetailId;
				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multifaqdetailEntity = _mapper.Map<Faqdetail>(model);
                        multifaqdetailEntity.LanguageId = item.LanguageId;
                        multifaqdetailEntity.CreatedBy = userinId;
						multifaqdetailEntity.Created = DateTime.Now;
                        multifaqdetailEntity.OrderNo = 0;
                        await _unitOfWork.FaqDetails.Create(multifaqdetailEntity);
					}
					await _unitOfWork.FaqDetails.Commit();
					return StatusCode(StatusCodes.Status201Created);
				}

				var faqdetailEntity = _mapper.Map<Faqdetail>(model);
				faqdetailEntity.CreatedBy = userinId;
				faqdetailEntity.Created = DateTime.Now;
                faqdetailEntity.OrderNo = 0;
                await _unitOfWork.FaqDetails.Create(faqdetailEntity);

				await _unitOfWork.FaqDetails.Commit();

				return StatusCode(StatusCodes.Status201Created, faqdetailEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateFaqDetail")]
		public async Task<IActionResult> UpdateFaqDetail([FromBody] FaqDetailDto model)
		{
			var faqdetailEntity = await _unitOfWork.FaqDetails.GetFaqDetailById(model.Id, model.LanguageId);

			if (faqdetailEntity == null)
			{
				return NotFound();
			}

			_mapper.Map(model, faqdetailEntity);
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
			faqdetailEntity.ModifiedBy = userinId;
			faqdetailEntity.Modified = DateTime.Now;

			_unitOfWork.FaqDetails.Update(faqdetailEntity);
			await _unitOfWork.FaqDetails.Commit();

			var menuToReturn = _mapper.Map<FaqDetailDto>(faqdetailEntity);
			return Ok(menuToReturn);

		}

		[Authorize]
		[HttpDelete("DeleteFaqDetail")]
		public async Task<IActionResult> DeleteFaqDetail(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var faqdetailId = await _unitOfWork.FaqDetails.GetFaqDetailById(id, webLangId);

			if (faqdetailId == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiContactEntity = await _unitOfWork.FaqDetails.GetFaqDetailById(id, item.Id);
					if (multiContactEntity != null)
					{
						_unitOfWork.FaqDetails.Delete(multiContactEntity);
						await _unitOfWork.FaqDetails.Commit();
					}
				}

				return Ok();
			}

			_unitOfWork.FaqDetails.Delete(faqdetailId);

			await _unitOfWork.FaqDetails.Commit();

			return Ok();
		}


        [Authorize]
        [HttpPost]
        [Route("UpdateFaqDetailsOrder")]
        public async Task<ActionResult> UpdateFaqDetailsMediaOrder(List<UpdateFaqDetailsOrderDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var faqdetails = await _unitOfWork.FaqDetails.GetDetailsById( item.Id, item.LanguageId, item.HeaderId);
                    if (faqdetails != null)
                    {
                        faqdetails.OrderNo = item.OrderNo;
                        _unitOfWork.FaqDetails.Update(faqdetails);
                    }
                }
                await _unitOfWork.FaqDetails.Commit();

                return Ok();

            }

            return NotFound();
        }


        #endregion
    }
}
