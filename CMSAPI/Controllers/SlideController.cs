using AutoMapper;
using CMS.API.InternalServices;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Abp.Linq.Expressions;
using System.Runtime.InteropServices;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SlideController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }
		private IFileRepository _fileRepository;
		public SlideController(
								IUnitOfWork unitOfWork
								, IMapper mapper
								, IFileRepository fileRepository
								)
		{
			_unitOfWork = unitOfWork;
			_fileRepository = fileRepository;
			_mapper = mapper;
		}

		[Authorize]
		[HttpGet]
		[Route("GetSlide")]
		public async Task<IActionResult> GetSlide(int webLangId = 1)
		{
			string[] includes = { "Layout", "Page", "Media" };
			var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			List<int> layoutIds = layouts.Select(x => x.Id).ToList();
            var slideQuery = PredicateBuilder.False<Slide>();

			foreach (var id in layoutIds)
			{
				slideQuery = slideQuery.Or(t => t.LayoutId == id);
			}
            slideQuery = slideQuery.And(t => t.LanguageId == webLangId);

            var List = await _unitOfWork.Slide.FindByCondition(slideQuery, false, includes).ToListAsync();



			if (List.Count > 0)
			{
				var listDto = _mapper.Map<IEnumerable<SlideListDto>?>(List);
				return Ok(listDto);
			}

			return NotFound();
		}

		[Authorize]
		[HttpPost]
		[Route("GetSlideAsync")]
		public async Task<IActionResult> GetSlideAsync([FromBody] FilterParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
			//List<int> layoutIds = layouts.Select(x => x.Id).ToList();
			var slide = await _unitOfWork.Slide.GetSlideAsync(parameter, false);
			var slideDto = _mapper.Map<IEnumerable<SlideListDto>?>(slide);

			return Ok(new
			{
				data = slideDto,
				total = slide.MetaData.TotalCount
			});
		}


		[Authorize]
		[HttpGet("GetSlideById")]
		public async Task<IActionResult> GetSlideById(int id, int webLangId)
		{
			var data = await _unitOfWork.Slide.GetSlideById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<getSlideDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateSlide")]
        public async Task<IActionResult> CreateSlide([FromForm] SlideDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image == null)
                {
                    ModelState.AddModelError("Image", " imazhi eshte fushe obligative");
                    return BadRequest(ModelState);
                }
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int slideId = _unitOfWork.Slide.GetMaxPK(i => i.Id);
                var langlist = await _unitOfWork.BaseConfig.GetLangList();
                var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
                model.Id = slideId;

                if (model.WebMultiLang)
                {
                    var mediaId = 0;
                    if (model.Image != null)
                    {
                        int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                        maxLength = maxLength * 1048576;
                        if (maxLength >= model.Image.Length)
                        {
                            var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                            //string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);
                            string filename = await _fileRepository.CropImages(model.Image, model.Width, model.Height);

                            Medium media = new Medium
                            {
                                MediaExCategoryId = 1,
                                Name = model.Image.FileName.Replace(fileEx, ""),
                                FileName = Guid.Parse(filename),
                                FileNameMedium = filename + "_medium",
                                FileNameSmall = filename + "_small",
                                FileEx = fileEx,
                                IsOtherSource = false,
                                CreatedBy = userinId,
                                Created = DateTime.Now
                            };
                            await _unitOfWork.Media.Create(media);
                            await _unitOfWork.Media.Commit();
                            mediaId = media.Id;
                        }
                    }
                    foreach (var item in langlist) //Insert per tri gjuhe
                    {
                        var slidesList = await _unitOfWork.Slide.FindByCondition(t => t.LanguageId == item.Id
                                                                                   && t.LayoutId == model.LayoutId && t.PageId == model.PageId && t.Deleted == false, false, null).ToListAsync();
                        if (slidesList.Where(t => t.OrderNo == 1).Count() > 0)
                        {
                            var slideListToReturn = _mapper.Map<List<UpdateSlideOrderDto>>(slidesList);
                            await UpdateSlideOrderBack(slideListToReturn);
                        }

                        var multiSlideEntity = _mapper.Map<Slide>(model);
                        multiSlideEntity.LanguageId = item.Id;
                        multiSlideEntity.CreatedBy = userinId;
                        multiSlideEntity.Created = DateTime.Now;
                        multiSlideEntity.MediaId = mediaId;
                        multiSlideEntity.OrderNo = 1;
                        await _unitOfWork.Slide.Create(multiSlideEntity);
                        await _unitOfWork.Slide.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created, new { Id = slideId });
                }

                var slidesListOneLang = await _unitOfWork.Slide.FindByCondition(t => t.LanguageId == model.LanguageId
                                                                           && t.LayoutId == model.LayoutId && t.PageId == model.PageId && t.Deleted == false, false, null).ToListAsync();
                if (slidesListOneLang.Where(t => t.OrderNo == 1).Count() > 0)
                {
                    var slideListOneLangToReturn = _mapper.Map<List<UpdateSlideOrderDto>>(slidesListOneLang);
                    await UpdateSlideOrderBack(slideListOneLangToReturn);
                }
                var slideEntity = _mapper.Map<Slide>(model);
                slideEntity.CreatedBy = userinId;
                slideEntity.Created = DateTime.Now;
                if (model.Image != null)
                {
                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.Image.Length)
                    {
                        var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
                        string filename = await _fileRepository.CropImages(model.Image, model.Width, model.Height);

                        Medium media = new Medium
                        {
                            MediaExCategoryId = 1,
                            Name = model.Image.FileName.Replace(fileEx, ""),
                            FileName = Guid.Parse(filename),
                            FileNameMedium = filename + "_medium",
                            FileNameSmall = filename + "_small",
                            FileEx = fileEx,
                            IsOtherSource = false,
                            CreatedBy = userinId,
                            Created = DateTime.Now
                        };
                        await _unitOfWork.Media.Create(media);
                        await _unitOfWork.Media.Commit();
                        slideEntity.MediaId = media.Id;
                    }
                }

                await _unitOfWork.Slide.Create(slideEntity);

                await _unitOfWork.Slide.Commit();

                return StatusCode(StatusCodes.Status201Created, new { Id = slideId });


            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        private Medium CreateMedium(IFormFile image, string fileEx, string filename, string userId, int mediaExCategoryId)
        {
            return new Medium
            {
                MediaExCategoryId = mediaExCategoryId,
                Name = image.FileName.Replace(fileEx, ""),
                FileName = Guid.Parse(filename),
                FileNameMedium = mediaExCategoryId == 1 ? filename + "_medium" : filename,
                FileNameSmall = mediaExCategoryId == 1 ? filename + "_small" : filename,
                FileEx = fileEx,
                IsOtherSource = false,
                CreatedBy = userId,
                Created = DateTime.Now
            };
        }

        [Authorize]
		[HttpPut("UpdateSlide")]
        public async Task<IActionResult> UpdateSlide([FromForm] UpdateSlideDto model)
        {
            var slideEntity = await _unitOfWork.Slide.GetSlideById((int)model.Id, model.LanguageId);
            var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
            var userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var mediaEx = await _unitOfWork.Media.GetMediaEx();
            var fileEx = model.Image != null ? Path.GetExtension(model.Image.FileName).ToLower() : null;

            if (slideEntity == null)
            {
                int? pageId = null;

                if (model.PageId != null)
                {
                    var page = await _unitOfWork.Pages.GetPageById((int)model.PageId, model.LanguageId);
                    pageId = page?.Id;
                }

                slideEntity = _mapper.Map<Slide>(model);
                slideEntity.PageId = pageId;
                slideEntity.CreatedBy = userinId;
                slideEntity.Created = DateTime.Now;                  
                await _unitOfWork.Slide.Create(slideEntity);
            }
            else
            {
                _mapper.Map(model, slideEntity);

                if (model.Image != null)
                {
                    if (mediaEx.Contains(fileEx))
                    {
                        var mediaExCategory = await _unitOfWork.Media.GetMediaExCategory(fileEx);

                        if (mediaExCategory.MediaExCategoryId == 1) // Photo
                        {
                            int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value) * 1048576;

                            if (maxLength >= model.Image.Length && model.Width != null && model.Height != null)
                            {
                                string filename = await _fileRepository.CropImages(model.Image, model.Width.Value, model.Height.Value);
                                Medium media = CreateMedium(model.Image, fileEx, filename, userinId, mediaExCategory.MediaExCategoryId);
                                await _unitOfWork.Media.Create(media);
                                await _unitOfWork.Media.Commit();
                                slideEntity.MediaId = media.Id;
                            }
                        }
                        else if (mediaExCategory.MediaExCategoryId == 2) // Video
                        {
                            int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxVideoSize").Value) * 1048576;

                            if (maxLength >= model.Image.Length)
                            {
                                string filename = await _fileRepository.AddFileAndVideo(model.Image, mediaExCategory.MediaExCategoryId);
                                Medium media = CreateMedium(model.Image, fileEx, filename, userinId, mediaExCategory.MediaExCategoryId);
                                await _unitOfWork.Media.Create(media);
                                await _unitOfWork.Media.Commit();
                                slideEntity.MediaId = media.Id;
                            }
                            else
                            {
                                return BadRequest($"Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!");
                            }
                        }
                    }
                }
                else
                {
                    slideEntity.MediaId = model.MediaId;
                }

                slideEntity.ModifiedBy = userinId;
                slideEntity.Modified = DateTime.Now;

                _unitOfWork.Slide.Update(slideEntity);
            }

            await _unitOfWork.Slide.Commit();

            var modelToReturn = _mapper.Map<SlideDto>(slideEntity);
            return Ok(modelToReturn);
        }		        

        [Authorize]
		[HttpDelete("DeleteSlide")]
		public async Task<IActionResult> DeleteSlide(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var slide = await _unitOfWork.Slide.GetSlideById(id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (slide == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiSlideEntity = await _unitOfWork.Slide.GetSlideById(id, item.Id);
					if (multiSlideEntity != null)
					{
						multiSlideEntity.Deleted = true;
						multiSlideEntity.Modified = DateTime.Now;
						multiSlideEntity.ModifiedBy = userId;
						_unitOfWork.Slide.Update(multiSlideEntity);
						await _unitOfWork.Slide.Commit();
					}
				}

				return Ok();
			}

			slide.Deleted = true;
			slide.Modified = DateTime.Now;
			slide.ModifiedBy = userId;
			_unitOfWork.Slide.Update(slide);

			await _unitOfWork.Slide.Commit();

			return Ok();
		}

		[Authorize]
		[HttpPost]
		[Route("UpdateSlideOrder")]
		public async Task<ActionResult> UpdateSlideOrder(List<UpdateSlideOrderDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var slide = await _unitOfWork.Slide.GetSlideById(item.Id, item.LanguageId);
					if (slide != null)
					{
						slide.OrderNo = item.OrderNo;
						_unitOfWork.Slide.Update(slide);
						await _unitOfWork.Slide.Commit();
					}
				}
				return Ok();
			}

			return NotFound();
		}
        private async Task<bool> UpdateSlideOrderBack(List<UpdateSlideOrderDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var slide = await _unitOfWork.Slide.GetSlideById(item.Id, item.LanguageId);
                    if (slide != null)
                    {
                        slide.OrderNo = item.OrderNo + 1;
                        _unitOfWork.Slide.Update(slide);
                        await _unitOfWork.Slide.Commit();
                    }
                }
                return true;
            }

            return false;
        }

        [Authorize]
		[HttpPost]
		[Route("CreateSlideOtherSource")]
		public async Task<IActionResult> CreateSlideOtherSource([FromBody] SlideOtherSourceDto model)
		{
			if (ModelState.IsValid)
			{
				var langlist = await _unitOfWork.BaseConfig.GetLangList();
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int slideId = _unitOfWork.Slide.GetMaxPK(i => i.Id);
				var mediaId = 0;
				if (model.WebMultiLang)
				{
					var mediumOtherSource = _mapper.Map<Medium>(model);

					mediumOtherSource.Name = model.Title;
					mediumOtherSource.FileName = Guid.NewGuid();
					mediumOtherSource.IsOtherSource = true;
					mediumOtherSource.CreatedBy = userinId;
					mediumOtherSource.Created = DateTime.Now;

					await _unitOfWork.Media.Create(mediumOtherSource);
					await _unitOfWork.Media.Commit();
					mediaId = mediumOtherSource.Id;

					foreach (var item in langlist) //Insert per tri gjuhe
					{
						var multiSlideOtherSource = _mapper.Map<Slide>(model);
						multiSlideOtherSource.Id = slideId;
						multiSlideOtherSource.LanguageId = item.Id;
						multiSlideOtherSource.CreatedBy = userinId;
						multiSlideOtherSource.Created = DateTime.Now;
						multiSlideOtherSource.MediaId = mediaId;
						await _unitOfWork.Slide.Create(multiSlideOtherSource);
						await _unitOfWork.Slide.Commit();
					}
				}
				else
				{
					var mediumOtherSource = _mapper.Map<Medium>(model);

					mediumOtherSource.Name = model.Title;
					mediumOtherSource.FileName = Guid.NewGuid();
					mediumOtherSource.IsOtherSource = true;
					mediumOtherSource.CreatedBy = userinId;
					mediumOtherSource.Created = DateTime.Now;

					await _unitOfWork.Media.Create(mediumOtherSource);
					await _unitOfWork.Media.Commit();

					var slideOtherSource = _mapper.Map<Slide>(model);
					slideOtherSource.Id = slideId;
					slideOtherSource.LanguageId = model.LanguageId;
					slideOtherSource.CreatedBy = userinId;
					slideOtherSource.Created = DateTime.Now;
					slideOtherSource.MediaId = mediumOtherSource.Id;
					await _unitOfWork.Slide.Create(slideOtherSource);
					await _unitOfWork.Slide.Commit();
				}

				return StatusCode(StatusCodes.Status201Created);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		#region Trash

		[Authorize]
		[HttpPost]
		[Route("GetSlideTrashAsync")]
		public async Task<IActionResult> GetSlideTrashAsync([FromBody] FilterParameters parameter)
		{
			var slide = await _unitOfWork.Slide.GetSlideAsync(parameter, true);
			var slideDto = _mapper.Map<IEnumerable<SlideListDto>?>(slide);

			return Ok(new
			{
				data = slideDto,
				total = slide.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpDelete("DeleteSlideFromTrash")]
		public async Task<IActionResult> DeleteSlideFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var slide = await _unitOfWork.Slide.GetSlideById(id, webLangId);

			if (slide == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiSlideEntity = await _unitOfWork.Slide.GetSlideById(id, item.Id);
					if (multiSlideEntity != null)
					{
						_unitOfWork.Slide.Delete(multiSlideEntity);
						await _unitOfWork.Slide.Commit();
					}
				}

				return Ok();
			}

			_unitOfWork.Slide.Delete(slide);

			await _unitOfWork.Slide.Commit();

			return Ok();
		}

		[Authorize]
		[HttpPut("RestoreSlideFromTrash")]
		public async Task<IActionResult> RestoreSlideFromTrash(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var slide = await _unitOfWork.Slide.GetSlideById(id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (slide == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiSlideEntity = await _unitOfWork.Slide.GetSlideById(id, item.Id);
					if (multiSlideEntity != null)
					{
						multiSlideEntity.Deleted = false;
						multiSlideEntity.Modified = DateTime.Now;
						multiSlideEntity.ModifiedBy = userId;
						_unitOfWork.Slide.Update(multiSlideEntity);
						await _unitOfWork.Slide.Commit();
					}
				}

				return Ok();
			}

			slide.Deleted = false;
			slide.Modified = DateTime.Now;
			slide.ModifiedBy = userId;
			_unitOfWork.Slide.Update(slide);

			await _unitOfWork.Slide.Commit();

			return Ok();
		}
		#endregion
	}
}
