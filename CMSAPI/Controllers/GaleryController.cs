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

namespace CMS.API.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class GaleryController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }

		public GaleryController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		#region GaleryCategory

		[Authorize]
		[HttpGet]
		[Route("GetGaleryCategory")]
		public async Task<IActionResult> GetGaleryCategory()
		{
			var galeryCategoryList = await _unitOfWork.GaleryCategories.FindAll(false, null).IgnoreAutoIncludes().ToListAsync();
			return Ok(galeryCategoryList);
		}

		#endregion

		#region GaleryHeader

		[Authorize]
		[HttpGet]
		[Route("GetGaleryHeaders")]
		public async Task<IActionResult> GetGaleryHeaders(int webLangId = 1)
		{
			string[] includes = { "Layout", "GaleryDetails" };
			var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var GaleryHeaderList = await _unitOfWork.GaleryHeaders.FindByCondition(t => t.LanguageId == webLangId
																					 && layoutIds.Contains(t.LayoutId), false, includes).ToListAsync();


			if (GaleryHeaderList.Count > 0)
			{
				var GaleryHeadersDto = _mapper.Map<IEnumerable<GaleryHeaderListDto>?>(GaleryHeaderList);
				return Ok(GaleryHeadersDto);
			}

			return NotFound();
		}

		[Authorize]
		[HttpPost]
		[Route("GetGaleryHeadersAsync")]
		public async Task<IActionResult> GetGaleryHeadersAsync([FromBody] GaleryFilterParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			//List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var GaleryHeader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderAsync(parameter, false);
			var GaleryHeadersDto = _mapper.Map<IEnumerable<GaleryHeaderListDto>?>(GaleryHeader);

			return Ok(new
			{
				data = GaleryHeadersDto,
				total = GaleryHeader.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetGaleryHeaderById")]
		public async Task<IActionResult> GetGaleryHeaderById(int id, int webLangId)
		{
			var data = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<GaleryHeaderDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateGaleryHeader")]
		public async Task<IActionResult> CreateGaleryHeader([FromBody] GaleryHeaderDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int GaleryHeaderId = _unitOfWork.GaleryHeaders.GetMaxPK(a => a.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = GaleryHeaderId;
				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
                        var GaleryHeaderList = await _unitOfWork.GaleryHeaders.FindByCondition(t => t.LanguageId == item.Id
                                                                                   && t.LayoutId == model.LayoutId && t.IsDeleted == false, false, null).ToListAsync();
                        if (GaleryHeaderList.Where(t => t.OrderNo == 1).Count() > 0)
                        {
                            var galeryHeaderToReturn = _mapper.Map<List<UpdateGaleryHeaderOrderDto>>(GaleryHeaderList);
                            await UpdateGaleryHeaderOrderBack(galeryHeaderToReturn);
                        }
                        var multiGaleryHeaderEntity = _mapper.Map<GaleryHeader>(model);
						multiGaleryHeaderEntity.LanguageId = item.Id;
						multiGaleryHeaderEntity.CreatedBy = userinId;
						multiGaleryHeaderEntity.Created = DateTime.Now;
                        multiGaleryHeaderEntity.OrderNo = 1;
                        await _unitOfWork.GaleryHeaders.Create(multiGaleryHeaderEntity);
						await _unitOfWork.GaleryHeaders.Commit();
					}

					return StatusCode(StatusCodes.Status201Created, new { Id = GaleryHeaderId });
				}

                var GaleryHeaderListOneLang = await _unitOfWork.GaleryHeaders.FindByCondition(t => t.LanguageId == model.LanguageId
                                                                         && t.LayoutId == model.LayoutId, false, null).ToListAsync();
                if (GaleryHeaderListOneLang.Where(t => t.OrderNo == 1).Count() > 0)
                {
                    var galeryHeaderToReturnOneLang = _mapper.Map<List<UpdateGaleryHeaderOrderDto>>(GaleryHeaderListOneLang);
                    await UpdateGaleryHeaderOrderBack(galeryHeaderToReturnOneLang);
                }
                var GaleryHeaderEntity = _mapper.Map<GaleryHeader>(model);
				GaleryHeaderEntity.CreatedBy = userinId;
				GaleryHeaderEntity.Created = DateTime.Now;
                GaleryHeaderEntity.OrderNo = 1;
                await _unitOfWork.GaleryHeaders.Create(GaleryHeaderEntity);

				await _unitOfWork.GaleryHeaders.Commit();

				return StatusCode(StatusCodes.Status201Created, new { Id = GaleryHeaderId });
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateGaleryHeader")]
		public async Task<IActionResult> UpdateGaleryHeader([FromBody] GaleryHeaderDto model)
		{
			var GaleryHeaderEntity = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(model.Id, model.LanguageId);

			if (GaleryHeaderEntity == null)
			{
				GaleryHeaderEntity = _mapper.Map<GaleryHeader>(model);
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				GaleryHeaderEntity.CreatedBy = userinId;
				GaleryHeaderEntity.Created = DateTime.Now;
				await _unitOfWork.GaleryHeaders.Create(GaleryHeaderEntity);
			}
			else
			{
				_mapper.Map(model, GaleryHeaderEntity);
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				GaleryHeaderEntity.ModifiedBy = userinId;
				GaleryHeaderEntity.Modified = DateTime.Now;
				_unitOfWork.GaleryHeaders.Update(GaleryHeaderEntity);
			}

			await _unitOfWork.GaleryHeaders.Commit();

			var FaqHeaderToReturn = _mapper.Map<GaleryHeaderDto>(GaleryHeaderEntity);

			return Ok(FaqHeaderToReturn);
		}

		[Authorize]
		[HttpDelete("DeleteGaleryHeader")]
		public async Task<IActionResult> DeleteGaleryHeader(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var GaleryHeader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (GaleryHeader == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiGaleryHeaderEntity = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, item.Id);
					if (multiGaleryHeaderEntity != null)
					{
						multiGaleryHeaderEntity.IsDeleted = true;
						multiGaleryHeaderEntity.Modified = DateTime.Now;
						multiGaleryHeaderEntity.ModifiedBy = userId;
						_unitOfWork.GaleryHeaders.Update(multiGaleryHeaderEntity);
						await _unitOfWork.GaleryHeaders.Commit();
					}
				}
			}
			else
			{
				GaleryHeader.IsDeleted = true;
				GaleryHeader.Modified = DateTime.Now;
				GaleryHeader.ModifiedBy = userId;

				_unitOfWork.GaleryHeaders.Update(GaleryHeader);

				await _unitOfWork.GaleryHeaders.Commit();
			}
			return Ok(new { status = true });
		}

		[Authorize]
		[HttpPost]
		[Route("UpdateGaleryHeaderOrder")]
		public async Task<ActionResult> UpdateGaleryHeaderOrder(List<UpdateGaleryHeaderOrderDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var galeryheader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(item.Id, item.LanguageId);
					if (galeryheader != null)
					{
						galeryheader.OrderNo = item.OrderNo;
						_unitOfWork.GaleryHeaders.Update(galeryheader);
					}
				}
				await _unitOfWork.GaleryHeaders.Commit();

				return Ok();

			}

			return NotFound();
		}

        private async Task<bool> UpdateGaleryHeaderOrderBack(List<UpdateGaleryHeaderOrderDto>? model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var galeryheader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(item.Id, item.LanguageId);
                    if (galeryheader != null)
                    {
                        galeryheader.OrderNo = item.OrderNo + 1;
                        _unitOfWork.GaleryHeaders.Update(galeryheader);
                    }
                }
                await _unitOfWork.GaleryHeaders.Commit();
                return true;
            }
            return false;
        }

        #endregion

        #region GaleryDetail


        [Authorize]
		[HttpGet]
		[Route("GetGaleryDetails")]
		//int categoryId,
		public async Task<IActionResult> GetGaleryDetails(int? GaleryHeaderId, int webLangId = 1)
		{
			string[] includes = { "GaleryHeader", "Media" }; //&& t.Media.MediaExCategoryId == categoryId
			var categoryDetailList = await _unitOfWork.GaleryDetails.FindByCondition(t => t.LanguageId == webLangId && t.HeaderId == GaleryHeaderId, false, includes).OrderBy(x => x.OrderNo).ToListAsync();
			var categorydetailsDto = _mapper.Map<IEnumerable<GaleryDetailListDto>?>(categoryDetailList);
			return Ok(categorydetailsDto);

		}

		[Authorize]
		[HttpPost]
		[Route("GetGaleryDetailsAsync")]
		public async Task<IActionResult> GetGaleryDetailsAsync([FromBody] GaleryDetailParameters parameter)
		{
			var galeryDetail = await _unitOfWork.GaleryDetails.GetGaleryDetailAsync(parameter);
			var galeryDetailsDto = _mapper.Map<IEnumerable<GaleryDetailListDto>?>(galeryDetail);

			return Ok(new
			{
				data = galeryDetailsDto,
				total = galeryDetail.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpPost]
		[Route("CreateGaleryDetail")]
		public async Task<IActionResult> CreateGaleryDetail([FromBody] GaleryDetailDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				if (model.WebMultiLang)
				{
					var langList = _unitOfWork.GaleryHeaders.FindByCondition(x => x.Id == model.HeaderId, false, null);
					foreach (var itemMedia in model.MediaIds)
					{
						int galerydetailId = _unitOfWork.GaleryDetails.GetMaxPK(i => i.Id);
						foreach (var item in langList)
						{
							GaleryDetail multigalerydetailEntity = new GaleryDetail();
							multigalerydetailEntity.Id = galerydetailId;
							multigalerydetailEntity.HeaderId = model.HeaderId;
							multigalerydetailEntity.MediaId = itemMedia;
							multigalerydetailEntity.LanguageId = item.LanguageId;
							multigalerydetailEntity.CreatedBy = userinId;
							multigalerydetailEntity.Created = DateTime.Now;

							await _unitOfWork.GaleryDetails.Create(multigalerydetailEntity);
						}

						await _unitOfWork.GaleryDetails.Commit();
					}
					return StatusCode(StatusCodes.Status201Created);
				}
				else
				{
					foreach (var itemMedia in model.MediaIds)
					{
						int galerydetailId = _unitOfWork.GaleryDetails.GetMaxPK(i => i.Id);

						GaleryDetail multigalerydetailEntity = new GaleryDetail();
						multigalerydetailEntity.Id = galerydetailId;
						multigalerydetailEntity.HeaderId = model.HeaderId;
						multigalerydetailEntity.MediaId = itemMedia;
						multigalerydetailEntity.LanguageId = model.webLangId;
						multigalerydetailEntity.CreatedBy = userinId;
						multigalerydetailEntity.Created = DateTime.Now;

						await _unitOfWork.GaleryDetails.Create(multigalerydetailEntity);
						await _unitOfWork.GaleryDetails.Commit();
					}
					return StatusCode(StatusCodes.Status201Created);
				}
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpDelete("DeleteGaleryDetail")]
		public async Task<IActionResult> DeleteGaleryDetail(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var galeryDetailId = await _unitOfWork.GaleryDetails.GetGaleryDetailById(id, webLangId);

			if (galeryDetailId == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiGaleryDetailEntity = await _unitOfWork.GaleryDetails.GetGaleryDetailById(id, item.Id);
					if (multiGaleryDetailEntity != null)
					{
						_unitOfWork.GaleryDetails.Delete(multiGaleryDetailEntity);
						await _unitOfWork.GaleryDetails.Commit();
					}
				}

				return Ok();
			}

			_unitOfWork.GaleryDetails.Delete(galeryDetailId);

			await _unitOfWork.GaleryDetails.Commit();

			return Ok();
		}

		[Authorize]
		[HttpPost]
		[Route("UpdateGaleryMediaOrder")]
		public async Task<ActionResult> UpdateGaleryMediaOrder(List<UpdateGaleryMediaOrderDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var galeryMedia = await _unitOfWork.GaleryDetails.GetMediaById(item.MediaId, item.LanguageId, item.HeaderId);
					if (galeryMedia != null)
					{
						galeryMedia.OrderNo = item.OrderNo;
						_unitOfWork.GaleryDetails.Update(galeryMedia);
					}
				}
				await _unitOfWork.GaleryDetails.Commit();

				return Ok();

			}

			return NotFound();
		}

		#endregion

		#region Trash Galery Header

		[Authorize]
		[HttpPost]
		[Route("GetGaleryHeadersTrashAsync")]
		public async Task<IActionResult> GetGaleryHeadersTrashAsync([FromBody] GaleryFilterParameters parameter)
		{
			var GaleryHeader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderAsync(parameter, true);
			var GaleryHeadersDto = _mapper.Map<IEnumerable<GaleryHeaderListDto>?>(GaleryHeader);

			return Ok(new
			{
				data = GaleryHeadersDto,
				total = GaleryHeader.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpPut("RestoreGaleryHeaderFromTrash")]
		public async Task<IActionResult> RestoreGaleryHeaderFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var GaleryHeader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (GaleryHeader == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiGaleryHeaderEntity = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, item.Id);
					if (multiGaleryHeaderEntity != null)
					{
						multiGaleryHeaderEntity.IsDeleted = false;
						multiGaleryHeaderEntity.Modified = DateTime.Now;
						multiGaleryHeaderEntity.ModifiedBy = userId;
						_unitOfWork.GaleryHeaders.Update(multiGaleryHeaderEntity);
						await _unitOfWork.GaleryHeaders.Commit();
					}
				}
			}
			else
			{
				GaleryHeader.IsDeleted = false;
				GaleryHeader.Modified = DateTime.Now;
				GaleryHeader.ModifiedBy = userId;

				_unitOfWork.GaleryHeaders.Update(GaleryHeader);

				await _unitOfWork.GaleryHeaders.Commit();
			}
			return Ok(new { status = true });
		}

		[Authorize]
		[HttpDelete("DeleteGaleryHeaderFromTrash")]
		public async Task<IActionResult> DeleteGaleryHeaderFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var GaleryHeader = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, webLangId);

			if (GaleryHeader == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{

					var countDetails2 = _unitOfWork.GaleryDetails.FindAll(false, null).Where(x => x.HeaderId == id && x.LanguageId == item.Id);
					if (countDetails2.Any())
					{
						_unitOfWork.GaleryDetails.RemoveRange(countDetails2);
					}

					var multiGaleryHeaderEntity = await _unitOfWork.GaleryHeaders.GetGaleryHeaderById(id, item.Id);
					if (multiGaleryHeaderEntity != null)
					{
						_unitOfWork.GaleryHeaders.Delete(multiGaleryHeaderEntity);

					}
					await _unitOfWork.GaleryHeaders.Commit();

				}
			}
			else
			{

				var countDetails = _unitOfWork.GaleryDetails.FindAll(false, null).Where(x => x.HeaderId == id && x.LanguageId == webLangId);
				if (countDetails.Any())
				{
					_unitOfWork.GaleryDetails.RemoveRange(countDetails);
				}

				_unitOfWork.GaleryHeaders.Delete(GaleryHeader);

				await _unitOfWork.GaleryHeaders.Commit();
			}
			return Ok(new { status = true });
		}

		#endregion
	}
}
