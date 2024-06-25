using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PageController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }

		public PageController(
								IUnitOfWork unitOfWork
								, IMapper mapper
								)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		#region Page
		[Authorize]
		[HttpGet]
		[Route("GetPages")]
		public async Task<IActionResult> GetPages(int? LayoutId, int webLangId = 1)
		{
			string[] includes = { "Layout", "Template", "Media" };
			List<int> layoutIds = null;
			if (LayoutId == null || LayoutId == 0)
			{
				var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
				var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
				var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
				layoutIds = layouts.Select(x => x.Id).ToList();
			}
			var data = await _unitOfWork.Pages.FindByCondition(x => x.LanguageId == webLangId && x.Deleted != true && x.IsSubPage != true
			&& (LayoutId > 0 ? x.LayoutId == LayoutId : layoutIds.Contains(x.LayoutId)), false, null).ToListAsync();

			var dataDto = _mapper.Map<IEnumerable<PageJoinDto>?>(data);
			return Ok(dataDto);
		}

        [Authorize]
        [HttpGet]
        [Route("GetPagesWithContacts")]
        public async Task<IActionResult> GetPagesWithContacts(int? LayoutId, int webLangId = 1)
        {
            string[] includes = { "Layout", "Template", "Media" };
            List<int> layoutIds = null;
            if (LayoutId == null || LayoutId == 0)
            {
                var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
                var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
                layoutIds = layouts.Select(x => x.Id).ToList();
            }
            var pageContactsList = await _unitOfWork.Contacts.FindAll(false, null).Where(x => (LayoutId > 0 ? x.LayoutId == LayoutId : layoutIds.Contains(x.LayoutId))).Select(x => x.PageId).Distinct().ToListAsync();

            var pageList = _unitOfWork.Pages.FindByCondition(x => pageContactsList.Contains(x.Id) && x.Deleted != true && x.IsSubPage != true && x.LanguageId == webLangId, false, null);

            var dataDto = _mapper.Map<IEnumerable<PageJoinDto>?>(pageList);
            return Ok(dataDto);
        }

        [Authorize]
		[HttpPost]
		[Route("GetPagesAsync")]
		public async Task<IActionResult> GetPagesAsync([FromBody] PageFilterParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			//List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var pages = await _unitOfWork.Pages.GetPagesAsync(parameter);
			var pagesDto = _mapper.Map<IEnumerable<PageListDto>?>(pages);

			return Ok(new
			{
				data = pagesDto,
				total = pages.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpPost]
		[Route("GetSubPagesAsync")]
		public async Task<IActionResult> GetSubPagesAsync([FromBody] PageFilterParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			//List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var pages = await _unitOfWork.Pages.GetSubPagesAsync(parameter);
			var pagesDto = _mapper.Map<IEnumerable<PageListDto>?>(pages);

			return Ok(new
			{
				data = pagesDto,
				total = pages.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetPageById")]
		public async Task<IActionResult> GetPageById(int id, int webLangId = 1)
		{
			string[] includes = { "Media" };
			var data = await _unitOfWork.Pages.FindByCondition(t => t.Id == id && t.LanguageId == webLangId, false, includes).FirstOrDefaultAsync();

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<PageDto>(data);
				return Ok(data);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreatePage")]
		public async Task<IActionResult> CreatePage([FromBody] PageDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int pageId = _unitOfWork.Pages.GetMaxPK(a => a.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = pageId;
				int? LayoutId = model.LayoutId;
				if (model.PageParentId > 0 && model.LayoutId == null)
				{
					var subPageLayoutId = await _unitOfWork.Pages.FindByCondition(x => x.Id == (int)model.PageParentId, false, null).FirstOrDefaultAsync();
					LayoutId = subPageLayoutId.LayoutId;
				}

				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multiPageEntity = _mapper.Map<Page>(model);
						multiPageEntity.LayoutId = (int)LayoutId;
						multiPageEntity.LanguageId = item.Id;
						multiPageEntity.CreatedBy = userinId;
						multiPageEntity.Created = DateTime.Now;
						await _unitOfWork.Pages.Create(multiPageEntity);

						await _unitOfWork.Pages.Commit();
					}

					return StatusCode(StatusCodes.Status201Created, new { Id = pageId });
				}

				var pageEntity = _mapper.Map<Page>(model);
				pageEntity.LayoutId = (int)LayoutId;
				pageEntity.CreatedBy = userinId;
				pageEntity.Created = DateTime.Now;
				await _unitOfWork.Pages.Create(pageEntity);

				await _unitOfWork.Pages.Commit();

				return StatusCode(StatusCodes.Status201Created, new { Id = pageId });
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdatePage")]
		public async Task<IActionResult> UpdatePage([FromBody] PageDto model)
		{
			var pageEntity = await _unitOfWork.Pages.GetPageById(model.Id, model.LanguageId);


			int? LayoutId = model.LayoutId;
			if (model.PageParentId > 0 && model.LayoutId == null)
			{
				var subPageLayoutId = await _unitOfWork.Pages.FindByCondition(x => x.Id == (int)model.PageParentId, false, null).FirstOrDefaultAsync();
				LayoutId = subPageLayoutId.LayoutId;
			}

			if (pageEntity == null)
			{
				//Krijon page te ri ne gjuhen e caktuar pasi qe nuk ekziston
				pageEntity = _mapper.Map<Page>(model);

				pageEntity.LayoutId = (int)LayoutId;
				pageEntity.CreatedBy = _unitOfWork.BaseConfig.GetLoggedUserId();
				pageEntity.Created = DateTime.Now;
				await _unitOfWork.Pages.Create(pageEntity);
				await _unitOfWork.Pages.Commit();
			}
			else
			{
				_mapper.Map(model, pageEntity);

				pageEntity.LayoutId = (int)LayoutId;
				pageEntity.ModifiedBy = _unitOfWork.BaseConfig.GetLoggedUserId();
				pageEntity.Modified = DateTime.Now;
				_unitOfWork.Pages.Update(pageEntity);
				await _unitOfWork.Pages.Commit();
			}
			var menuToReturn = _mapper.Map<PageDto>(pageEntity);

			return Ok(menuToReturn);
		}

		[Authorize]
		[HttpDelete("DeletePage")]
		public async Task<IActionResult> DeletePage(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var page = await _unitOfWork.Pages.GetPageById(id, webLangId);

			if (page == null)
			{
				return NotFound();
			}

			if (WebMultiLang)
			{
				var multiPages = await _unitOfWork.Pages.FindByCondition(x => x.Id == id && x.Deleted == false, false, null).ToListAsync();
				foreach (var item in multiPages)
				{
					var post = await _unitOfWork.Pages.GetPageById(item.Id, item.LanguageId);
					post.Deleted = true;
					_unitOfWork.Pages.Update(post);
					await _unitOfWork.Pages.Commit();
				}

				return Ok();
			}

			var Singlepage = await _unitOfWork.Pages.GetPageById(id, webLangId);

			if (Singlepage == null)
			{
				return NotFound();
			}

			Singlepage.Deleted = true;

			_unitOfWork.Pages.Update(Singlepage);

			await _unitOfWork.Pages.Commit();

			return Ok();
		}

        [Authorize]
        [HttpGet("CheckPageBeforeDelete")]
        public async Task<IActionResult> CheckPageBeforeDelete(int id, int webLangId = 1)
        {
            var page = await _unitOfWork.Pages.GetPageById(id, webLangId);
            if (page == null)
            {
                return NotFound();
            }

            var lista = new List<CheckPageBeforeDeleteList>();
            var multiPages = await _unitOfWork.Pages.FindByCondition(x => x.Id == id && x.Deleted == false, false, new[] { "Language" }).ToListAsync();
            foreach (var item in multiPages)
            {
                var docCount = _unitOfWork.PageMedia.FindByCondition(x => x.PageId == id && x.LanguageId == item.LanguageId && x.IsSlider != true, false, null).Count();
                var mediaCount = _unitOfWork.PageMedia.FindByCondition(x => x.PageId == id && x.LanguageId == item.LanguageId && x.IsSlider == true, false, null).Count();
                var menuCount = _unitOfWork.Menu.FindByCondition(x => (x.PageId == id || x.PageParentId == id) && x.LanguageId == item.LanguageId && x.Active == true, false, null).Count();
                var subPageCount = _unitOfWork.Pages.FindByCondition(x => x.PageParentId == id && x.LanguageId == item.LanguageId && x.IsSubPage == true && x.Deleted != true, false, null).Count();
                if (docCount > 0 || mediaCount > 0 || menuCount > 0 || subPageCount > 0)
                {
                    lista.Add(new CheckPageBeforeDeleteList
                    {
                        Gjuha = item.Language.NameSq,
                        GjuhaId = item.Language.Id,
                        Documents = docCount,
                        Media = mediaCount,
                        Menus = menuCount,
                        SubPages = subPageCount
                    });
                }
            }

            return Ok(lista);
        }

        #endregion

        #region PageMedia
        [Authorize]
		[HttpPost]
		[Route("AddMediaCollectionInPage")]
		public async Task<ActionResult> AddMediaCollectionInPage(AddMediaCollectionInPage model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var langList = await _unitOfWork.BaseConfig.GetLangList();
					string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
					var result = await _unitOfWork.Pages.AddMediaCollectionInPage(model, langList, userinId);
					if (result)
						return Ok();
				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception)
			{
				return BadRequest(ModelState);
			}
			return BadRequest(ModelState);
		}

		[Authorize]
		[HttpDelete]
		[Route("RemoveMediaCollectionFromPage")]
		public async Task<ActionResult> RemoveMediaCollectionFromPage([FromQuery] string MediaIds, bool WebMultiLang, int webLangId, int pageId)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var langList = await _unitOfWork.BaseConfig.GetLangList();
					var numbers = MediaIds.Split(',')?.Select(Int32.Parse)?.ToList();
					if (numbers.Count > 0)
					{
						var result = await _unitOfWork.Pages.RemoveMediaCollectionFromPage(numbers, langList, WebMultiLang, webLangId, pageId);
						if (result)
							return Ok();
					}

				}
				else
				{
					return BadRequest(ModelState);
				}
			}
			catch (Exception)
			{
				return BadRequest(ModelState);
			}
			return BadRequest(ModelState);
		}

		[Authorize]
		[HttpGet]
		[Route("GetPageMedia")]
		public async Task<ActionResult> GetPageMedia(int pageId, bool isSlider, int webLangId = 1)
		{
			return Ok(await _unitOfWork.Pages.GetPageMedia(pageId, webLangId, isSlider));
		}

		[Authorize]
		[HttpPost]
		[Route("UpdatePageMediaOrder")]
		public async Task<ActionResult> UpdatePageMediaOrder(List<UpdatePageMediaOrderDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var pageMedia = await _unitOfWork.PageMedia.GetMediaById(item.MediaId, item.LanguageId, item.PageId);
					if (pageMedia != null)
					{
						pageMedia.OrderNo = item.OrderNo;
						_unitOfWork.PageMedia.Update(pageMedia);
					}
				}
				await _unitOfWork.PageMedia.Commit();

				return Ok();

			}

			return NotFound();
		}


		[Authorize]
		[HttpGet]
		[Route("GetMediabyId")]

		public async Task<ActionResult> GetMediabyId(int mediaId, int pageId, int webLangId = 1)
		{
			var media = await _unitOfWork.PageMedia.GetMediaById(mediaId, webLangId, pageId);

			return Ok(media);
		}



		[Authorize]
		[HttpPut("UpdateMediaInPage")]
		public async Task<IActionResult> UpdateMediaInPage([FromBody] UpdateMediaInPage model)
		{
			try
			{
				var pagemediaEntity = await _unitOfWork.PageMedia.GetMediaById(model.MediaId, model.LanguageId, model.PageId);
				DateTime dtstartDate = new();
				DateTime? dtEndDate = new();
				if (!string.IsNullOrEmpty(model.StartDate))
				{
					try
					{
						dtstartDate = _unitOfWork.BaseConfig.StringToDate(model.StartDate);
					}
					catch (Exception)
					{
						ModelState.AddModelError("StartDate", "Formati dates nuk eshte ne rregull");
					}
				}
				if (!string.IsNullOrEmpty(model.EndDate))
				{
					try
					{
						dtEndDate = _unitOfWork.BaseConfig.StringToDate(model.EndDate);
					}
					catch (Exception)
					{
						ModelState.AddModelError("EndDate", "Formati dates nuk eshte ne rregull");
					}
				}
				else
				{
					dtEndDate = null;
				}
				if (pagemediaEntity == null)
				{
					return NotFound();
				}

				//_mapper.Map(model, pagemediaEntity);
				//string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				//pagemediaEntity.StartDate = dtstartDate;
				//pagemediaEntity.EndDate = dtEndDate;
				//pagemediaEntity.ModifiedBy = userinId;
				//pagemediaEntity.Modified = DateTime.Now;
				//_unitOfWork.PageMedia.Update(pagemediaEntity);
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				pagemediaEntity.Name = model.Name;
				pagemediaEntity.Link = model.Link;
				pagemediaEntity.LinkName = model.LinkName;
				pagemediaEntity.StartDate = dtstartDate;
				pagemediaEntity.EndDate = dtEndDate;
				pagemediaEntity.ModifiedBy = userinId;
				pagemediaEntity.Modified = DateTime.Now;

				_unitOfWork.PageMedia.Update(pagemediaEntity);
				await _unitOfWork.PageMedia.Commit();

				var menuToReturn = _mapper.Map<UpdateMediaInPage>(pagemediaEntity);

				return Ok(menuToReturn);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		#endregion

		#region Trash Pages

		[Authorize]
		[HttpPost]
		[Route("GetPagesTrashAsync")]
		public async Task<IActionResult> GetPagesTrashAsync([FromBody] PageFilterParameters parameter)
		{
			parameter.isDeleted = true;
			var pages = await _unitOfWork.Pages.GetPagesAsync(parameter);
			var pagesDto = _mapper.Map<IEnumerable<PageListDto>?>(pages);

			return Ok(new
			{
				data = pagesDto,
				total = pages.MetaData.TotalCount
			});
		}


		[Authorize]
		[HttpDelete("DeletePageFromTrash")]
		public async Task<IActionResult> DeletePageFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var page = await _unitOfWork.Pages.GetPageById(id, webLangId);

			if (page == null)
			{
				return NotFound();
			}

			if (WebMultiLang)
			{
				var multiPages = await _unitOfWork.Pages.FindByCondition(x => x.Id == id && x.Deleted == true, false, null).ToListAsync();
				foreach (var item in multiPages)
				{
					var pageMedia = _unitOfWork.PageMedia.FindByCondition(x => x.PageId == item.Id && x.LanguageId == item.LanguageId, false, null);
					if (pageMedia.Any())
					{
						_unitOfWork.PageMedia.RemoveRange(pageMedia);
					}

					//fshierja nga menyja
					var pagesMenu = _unitOfWork.Menu.FindByCondition(x => (x.PageId == item.Id || x.PageParentId == item.Id) && x.LanguageId == item.LanguageId, false, null);
					if (pagesMenu.Any())
					{
						_unitOfWork.Menu.RemoveRange(pagesMenu);
					}

					var post = await _unitOfWork.Pages.GetPageById(item.Id, item.LanguageId);
					if (post != null)
					{
						_unitOfWork.Pages.Delete(post);
					}

					await _unitOfWork.Pages.Commit();
				}
				return Ok();
			}
			var Singlepage = await _unitOfWork.Pages.GetPageById(id, webLangId);

			if (Singlepage == null)
			{
				return NotFound();
			}

			var SinglepageMedia = _unitOfWork.PageMedia.FindByCondition(x => x.PageId == id && x.LanguageId == webLangId, false, null);
			if (SinglepageMedia.Any())
			{
				_unitOfWork.PageMedia.RemoveRange(SinglepageMedia);
			}

			//fshierja nga menyja
			var pagesMenu1 = _unitOfWork.Menu.FindByCondition(x => (x.PageId == id || x.PageParentId == id) && x.LanguageId == webLangId, false, null);
			if (pagesMenu1.Any())
			{
				_unitOfWork.Menu.RemoveRange(pagesMenu1);
			}

			_unitOfWork.Pages.Delete(Singlepage);

			await _unitOfWork.Pages.Commit();

			return Ok();
		}


		[Authorize]
		[HttpPut("RestorePageFromTrash")]
		public async Task<IActionResult> RestorePageFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var page = await _unitOfWork.Pages.GetPageById(id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (page == null)
			{
				return NotFound();
			}

			if (WebMultiLang)
			{
				var multiPages = await _unitOfWork.Pages.FindByCondition(x => x.Id == id, false, null).ToListAsync();
				foreach (var item in multiPages)
				{
					var post = await _unitOfWork.Pages.GetPageById(item.Id, item.LanguageId);
					post.Deleted = false;
					post.Modified = DateTime.Now;
					post.ModifiedBy = userId;
					_unitOfWork.Pages.Update(post);
					await _unitOfWork.Pages.Commit();
				}

				return Ok();
			}

			var Singlepage = await _unitOfWork.Pages.GetPageById(id, webLangId);

			if (Singlepage == null)
			{
				return NotFound();
			}

			Singlepage.Deleted = false;
			Singlepage.Modified = DateTime.Now;
			Singlepage.ModifiedBy = userId;

			_unitOfWork.Pages.Update(Singlepage);

			await _unitOfWork.Pages.Commit();

			return Ok();
		}

		#endregion

		#region Trash SubPages

		[Authorize]
		[HttpPost]
		[Route("GetSubPagesTrashAsync")]
		public async Task<IActionResult> GetSubPagesTrashAsync([FromBody] SubPageFilterParameters parameter)
		{
			parameter.isDeleted = true;
			var pages = await _unitOfWork.Pages.GetSubPagesTrashAsync(parameter);
			var pagesDto = _mapper.Map<IEnumerable<PageListDto>?>(pages);

			return Ok(new
			{
				data = pagesDto,
				total = pages.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet]
		[Route("GetParentSubpagesListTrash")]
		public async Task<IActionResult> GetParentSubpagesListTrash(int webLangId)
		{
			var subPageParentIds = await _unitOfWork.Pages.FindByCondition(x => x.IsSubPage == true && x.Deleted != false, false, null).Select(x => x.PageParentId).ToListAsync();

			var data = await _unitOfWork.Pages.FindByCondition(x => x.LanguageId == webLangId
			&& (subPageParentIds.Contains(x.Id)), false, null).ToListAsync();

			var dataDto = _mapper.Map<IEnumerable<PageJoinDto>?>(data);


			return Ok(dataDto);
		}

		#endregion

	}


}
