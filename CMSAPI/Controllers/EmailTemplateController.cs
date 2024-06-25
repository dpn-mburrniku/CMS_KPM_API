using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailTemplateController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;

		public IMapper _mapper { get; }
		public EmailTemplateController(IUnitOfWork unitOfWork, IMapper mapper)
		{

			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[Authorize]
		[HttpGet]
		[Route("GetEmailTemplates")]
		public async Task<IActionResult> GetEmailTemplates(int webLangId = 1)
		{
			var emailTemplateslList = await _unitOfWork.EmailTemplates.FindByCondition(t => t.LanguageId == webLangId, false, null).ToListAsync();

			var emailTemplatesDto = _mapper.Map<IEnumerable<EmailTemplateListDto>?>(emailTemplateslList);

			return Ok(emailTemplatesDto);

		}

		[Authorize]
		[HttpPost]
		[Route("GetEmailTemplatesAsync")]
		public async Task<IActionResult> GetEmailTemplatesAsync([FromBody] FilterParameters parameter)
		{
			var emailTemplates = await _unitOfWork.EmailTemplates.GetEmailTemplatesAsync(parameter);

			var emailTemplatesDto = _mapper.Map<List<EmailTemplateListDto>>(emailTemplates);

			return Ok(new
			{
				data = emailTemplatesDto,
				total = emailTemplates.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetEmailTemplateById")]
		public async Task<IActionResult> GetEmailTemplateById(int id, int webLangId)
		{
			var data = await _unitOfWork.EmailTemplates.GetEmailTemplateById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<EmailTemplateDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateEmailTemplate")]
		public async Task<IActionResult> CreateEmailTemplate([FromBody] EmailTemplateDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int emailTemplateId = _unitOfWork.EmailTemplates.GetMaxPK(a => a.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = emailTemplateId;

				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multiEmailTemplateEntity = _mapper.Map<EmailTemplate>(model);
						multiEmailTemplateEntity.LanguageId = item.Id;
						multiEmailTemplateEntity.CreatedBy = userinId;
						multiEmailTemplateEntity.Created = DateTime.Now;
						await _unitOfWork.EmailTemplates.Create(multiEmailTemplateEntity);

						await _unitOfWork.EmailTemplates.Commit();
					}

					return StatusCode(StatusCodes.Status201Created);
				}

				var emailTemplateEntity = _mapper.Map<EmailTemplate>(model);
				emailTemplateEntity.CreatedBy = userinId;
				emailTemplateEntity.Created = DateTime.Now;
				await _unitOfWork.EmailTemplates.Create(emailTemplateEntity);

				await _unitOfWork.EmailTemplates.Commit();

				return StatusCode(StatusCodes.Status201Created, emailTemplateEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateEmailTemplate")]
		public async Task<IActionResult> UpdateEmailTemplate([FromBody] EmailTemplateDto model)
		{
			var emailTemplateEntity = await _unitOfWork.EmailTemplates.GetEmailTemplateById(model.Id, model.LanguageId);

			if (emailTemplateEntity == null)
			{
				return NotFound();
			}

			_mapper.Map(model, emailTemplateEntity);
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
			emailTemplateEntity.ModifiedBy = userinId;
			emailTemplateEntity.Modified = DateTime.Now;
			_unitOfWork.EmailTemplates.Update(emailTemplateEntity);
			await _unitOfWork.EmailTemplates.Commit();

			var menuToReturn = _mapper.Map<EmailTemplateDto>(emailTemplateEntity);

			return Ok(menuToReturn);
		}

		[Authorize]
		[HttpDelete("DeleteEmailTemplate")]
		public async Task<IActionResult> DeleteEmailTemplate(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var emailTemplate = await _unitOfWork.EmailTemplates.GetEmailTemplateById(id, webLangId);

			if (emailTemplate == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var emailTemplateEntity = await _unitOfWork.EmailTemplates.GetEmailTemplateById(id, item.Id);
					if (emailTemplateEntity != null)
					{
						_unitOfWork.EmailTemplates.Delete(emailTemplateEntity);
						await _unitOfWork.EmailTemplates.Commit();
					}
				}
			}
			else
			{
				_unitOfWork.EmailTemplates.Delete(emailTemplate);

				await _unitOfWork.EmailTemplates.Commit();
			}
			return Ok();
		}

        #region EmailTemplateItems
        

        [Authorize]
		[HttpGet]
		[Route("GetEmailTemplateItems")]
		public async Task<IActionResult> GetEmailTemplateItems(int webLangId = 1, int emailTemplateId = 0)
		{


			string[] includes = { "EmailTemplate" };
			var emailTemplateitemslList = await _unitOfWork.EmailTemplateItem.FindByCondition(t => t.LanguageId == webLangId && t.EmailTemplateId == emailTemplateId, false, includes).ToListAsync();

			var emailTemplatesItemsDto = _mapper.Map<IEnumerable<EmailTemplateItemListDto>?>(emailTemplateitemslList);

			return Ok(emailTemplatesItemsDto);

		}

        [Authorize]
        [HttpPost]
        [Route("GetEmailTemplateItemsAsync")]
        public async Task<IActionResult> GetEmailTemplateItemsAsync([FromBody] EmailItemsFilterParameters parameter)
        {
            var emailTemplateItems = await _unitOfWork.EmailTemplateItem.GetEmailTemplateItemAsync(parameter);

			var emailTemplateItemsDto = _mapper.Map<List<EmailTemplateItemListDto>>(emailTemplateItems);

			return Ok(new
			{
				data = emailTemplateItemsDto,
				total = emailTemplateItems.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetEmailTemplatItemById")]
		public async Task<IActionResult> GetEmailTemplateItemById(int id, int webLangId)
		{
			var data = await _unitOfWork.EmailTemplateItem.GetEmailTemplateItemById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<EmailTemplateItemDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateEmailTemplateItem")]
		public async Task<IActionResult> CreateEmailTemplateItem([FromBody] EmailTemplateItemDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int emailTemplateItemId = _unitOfWork.EmailTemplateItem.GetMaxPK(a => a.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = emailTemplateItemId;

				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multiEmailTemplateItemEntity = _mapper.Map<EmailTemplateItem>(model);
						multiEmailTemplateItemEntity.LanguageId = item.Id;
						multiEmailTemplateItemEntity.CreatedBy = userinId;
						multiEmailTemplateItemEntity.Created = DateTime.Now;
						await _unitOfWork.EmailTemplateItem.Create(multiEmailTemplateItemEntity);

						await _unitOfWork.EmailTemplateItem.Commit();
					}

					return StatusCode(StatusCodes.Status201Created);
				}

				var emailTemplateItemEntity = _mapper.Map<EmailTemplateItem>(model);
				emailTemplateItemEntity.CreatedBy = userinId;
				emailTemplateItemEntity.Created = DateTime.Now;
				await _unitOfWork.EmailTemplateItem.Create(emailTemplateItemEntity);

				await _unitOfWork.EmailTemplateItem.Commit();

				return StatusCode(StatusCodes.Status201Created, emailTemplateItemEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateEmailTemplateItem")]
		public async Task<IActionResult> UpdateEmailTemplateItem([FromBody] EmailTemplateItemDto model)
		{
			var emailTemplateItemEntity = await _unitOfWork.EmailTemplateItem.GetEmailTemplateItemById(model.Id, model.LanguageId);

			if (emailTemplateItemEntity == null)
			{
				return NotFound();
			}

			_mapper.Map(model, emailTemplateItemEntity);
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
			emailTemplateItemEntity.ModifiedBy = userinId;
			emailTemplateItemEntity.Modified = DateTime.Now;
			_unitOfWork.EmailTemplateItem.Update(emailTemplateItemEntity);
			await _unitOfWork.EmailTemplateItem.Commit();

			var menuToReturn = _mapper.Map<EmailTemplateItemDto>(emailTemplateItemEntity);

			return Ok(menuToReturn);
		}

		[Authorize]
		[HttpDelete("DeleteEmailTemplateItem")]
		public async Task<IActionResult> DeleteEmailTemplateItem(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var emailTemplateItem = await _unitOfWork.EmailTemplateItem.GetEmailTemplateItemById(id, webLangId);

			if (emailTemplateItem == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var emailTemplateItemEntity = await _unitOfWork.EmailTemplateItem.GetEmailTemplateItemById(id, item.Id);
					if (emailTemplateItemEntity != null)
					{
						_unitOfWork.EmailTemplateItem.Delete(emailTemplateItemEntity);
						await _unitOfWork.EmailTemplateItem.Commit();
					}
				}
			}
			else
			{
				_unitOfWork.EmailTemplateItem.Delete(emailTemplateItem);

				await _unitOfWork.EmailTemplates.Commit();
			}
			return Ok();
		}

		#endregion
	}
}
