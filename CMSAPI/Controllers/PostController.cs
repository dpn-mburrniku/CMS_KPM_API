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
using System.Runtime.InteropServices;
using Abp.Linq.Expressions;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }

		public PostController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		#region Post
		[Authorize]
		[HttpGet]
		[Route("GetPosts")]
		public async Task<IActionResult> GetPosts(int webLangId = 1)
		{
			string[] includes = { "Layout", "Page" };
			var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });

            var postCategoriesQuery = PredicateBuilder.False<PostCategory>();

			foreach (var id in layouts.Select(x => x.Id))
			{
				postCategoriesQuery = postCategoriesQuery.Or(t => t.LayoutId == id);
			}
			postCategoriesQuery = postCategoriesQuery.And(t => t.LanguageId == webLangId && t.Active == true);

            var postCategoryList = await _unitOfWork.PostCategories.FindByCondition(postCategoriesQuery, false, includes).ToListAsync();


            var postsInCategoryQuery = PredicateBuilder.False<PostsInCategory>();

            foreach (var id in postCategoryList.Distinct().Select(x => x.Id).ToList())
            {
                postsInCategoryQuery = postsInCategoryQuery.Or(t => t.PostCategoryId == id);
            }
            postsInCategoryQuery = postsInCategoryQuery.And(t => t.LanguageId == webLangId);

            var postInCategories = _unitOfWork.PostsInCategory.FindByCondition(postsInCategoryQuery, false, null);

			List<int> postIds = postInCategories.Distinct().Select(x => x.PostId).ToList();


            var postQuery = PredicateBuilder.False<Post>();

            foreach (var id in postIds)
            {
                postQuery = postQuery.Or(t => t.Id == id);
            }
            postQuery = postQuery.And(t => t.LanguageId == webLangId);

            var PostsList = await _unitOfWork.Posts.FindByCondition(postQuery, false, new[] { "Media", "PostsInCategories", "PostsInCategories.PostCategory" }).IgnoreAutoIncludes().ToListAsync();
			
			var PostsDto = _mapper.Map<IEnumerable<PostListDto>?>(PostsList);
			return Ok(PostsDto);
		}

		[Authorize]
		[HttpPost]
		[Route("GetPostsAsync")]
		public async Task<IActionResult> GetPostsAsync([FromBody] PostFilterParameters parameter)
		{

			List<int>? postIds = null;
			string[] includes = { "Layout", "Page" };


			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });


			var postCategoryList = await _unitOfWork.PostCategories.FindAll(false, includes)
					.Where(t => t.LanguageId == parameter.webLangId && t.Id == (parameter.PostCategoryId > 0 ? parameter.PostCategoryId : t.Id)
					 && t.LayoutId == parameter.LayoutId).ToListAsync();
            //&& (parameter.LayoutId > 0 ? t.LayoutId == parameter.LayoutId : layouts.Select(x => x.Id).Contains(t.LayoutId))).ToListAsync();


            var postInCategoryQuery = PredicateBuilder.False<PostsInCategory>();

            foreach (var id in postCategoryList.Distinct().Select(x => x.Id))
            {
                postInCategoryQuery = postInCategoryQuery.Or(t => t.PostCategoryId == id);
            }
			postInCategoryQuery = postInCategoryQuery.And(x => x.LanguageId == parameter.webLangId);

            var postInCategories = _unitOfWork.PostsInCategory.FindByCondition(postInCategoryQuery, false, null);

			postIds = postInCategories.Distinct().Select(x => x.PostId).ToList();


			var posts = await _unitOfWork.Posts.GetPostsAsync(parameter, postIds, false);
			var postsDto = _mapper.Map<IEnumerable<PostListDto>?>(posts);

			return Ok(new
			{
				data = postsDto,
				total = posts.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetPostById")]
		public async Task<IActionResult> GetPostById(int id, int webLangId)
		{
			var data = await _unitOfWork.Posts.FindByCondition(t => t.Id == id && t.LanguageId == webLangId, false, new[] { "Media", "PostsInCategories", "PostsInCategories.PostCategory", "PostsInTags", "PostsInTags.PostTag", "PostsInCategories.PostCategory.Layout" }).FirstOrDefaultAsync();

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<PostListDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreatePost")]

		public async Task<IActionResult> CreatePost([FromBody] PostDto model)
		{
			DateTime dtstartDate = new();
			DateTime? dtEndDate = new();
			DateTime? dteventDate = new();
			if (!string.IsNullOrEmpty(model.StartDateStr))
			{
				try
				{
					dtstartDate = _unitOfWork.BaseConfig.StringToDateTime(model.StartDateStr);
				}
				catch (Exception)
				{
					ModelState.AddModelError("StartDate", "Formati dates nuk eshte ne rregull");
				}
			}
			if (!string.IsNullOrEmpty(model.EndDateStr))
			{
				try
				{
					dtEndDate = _unitOfWork.BaseConfig.StringToDateTime(model.EndDateStr);
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
			if (!string.IsNullOrEmpty(model.EventDateStr))
			{
				try
				{
					dteventDate = _unitOfWork.BaseConfig.StringToDateTime(model.EventDateStr);
				}
				catch (Exception)
				{
				}
			}
			else
			{
				dteventDate = null;
			}


			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int PostId = _unitOfWork.Posts.GetMaxPK(i => i.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = PostId;

				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multipostEntity = _mapper.Map<Post>(model);
						multipostEntity.LanguageId = item.Id;
						multipostEntity.CreatedBy = userinId;
						multipostEntity.Created = DateTime.Now;
						multipostEntity.StartDate = dtstartDate;
						multipostEntity.EndDate = dtEndDate;
						multipostEntity.EventDate = dteventDate;

						await _unitOfWork.Posts.Create(multipostEntity);
						await _unitOfWork.Posts.Commit();

                        var postCategoriesQuery = PredicateBuilder.False<PostCategory>();
                        foreach (var id in model.PostCategoryIds)
                        {
                            postCategoriesQuery = postCategoriesQuery.Or(t => t.Id == id);
                        }
                        postCategoriesQuery = postCategoriesQuery.And(t => t.LanguageId == item.Id);

                        var _categoryIds = await _unitOfWork.PostCategories.FindByCondition(postCategoriesQuery, false, null).ToListAsync(); 
						foreach (var catId in _categoryIds)
						{
							var postsInCategoryEntity = new PostsInCategory()
							{
								PostCategoryId = catId.Id,
								PostId = model.Id,
								LanguageId = item.Id,
								CreatedBy = userinId,
								Created = DateTime.Now,
							};

							await _unitOfWork.PostsInCategory.Create(postsInCategoryEntity);
							await _unitOfWork.Posts.Commit();
						}

						if (model.PostTagsIds != null)
                        {
                            var postTagsQuery = PredicateBuilder.False<PostTag>();
                            foreach (var id in model.PostCategoryIds)
                            {
                                postTagsQuery = postTagsQuery.Or(t => t.Id == id);
                            }
                            postTagsQuery = postTagsQuery.And(t => t.LanguageId == item.Id);

                            var _tagsIds = await _unitOfWork.Tags.FindByCondition(postTagsQuery, false, null).ToListAsync();
                            foreach (var catId in _tagsIds)
                            {
                                var postsInTagsEntity = new PostsInTag()
                                {
                                    PostTagId = catId.Id,
                                    LanguageId = item.Id,
                                    PostId = model.Id,
                                    CreatedBy = userinId,
                                    Created = DateTime.Now,
                                };

                                await _unitOfWork.PostsInTags.Create(postsInTagsEntity);
                                await _unitOfWork.Posts.Commit();
                            }
                        }
                       
                    }
					return StatusCode(StatusCodes.Status201Created, new { Id = PostId });
				}

				var PostEntity = _mapper.Map<Post>(model);
				PostEntity.StartDate = dtstartDate;
				PostEntity.EndDate = dtEndDate;
				PostEntity.EventDate = dteventDate;
				PostEntity.CreatedBy = userinId;
				PostEntity.Created = DateTime.Now;
				await _unitOfWork.Posts.Create(PostEntity);

				await _unitOfWork.Posts.Commit();


				foreach (var catId in model.PostCategoryIds)
				{
					var postsInCategoryEntity = new PostsInCategory()
					{
						PostCategoryId = catId,
						PostId = model.Id,
						LanguageId = model.LanguageId,
						CreatedBy = userinId,
						Created = DateTime.Now,
					};

					await _unitOfWork.PostsInCategory.Create(postsInCategoryEntity);
					await _unitOfWork.Posts.Commit();
				}

                foreach (var catId in model.PostTagsIds)
                {
                    var postsInTagsEntity = new PostsInTag()
                    {
                        PostTagId = catId,
                        LanguageId = model.LanguageId,
                        PostId = model.Id,
                        CreatedBy = userinId,
                        Created = DateTime.Now,
                    };

                    await _unitOfWork.PostsInTags.Create(postsInTagsEntity);
                    await _unitOfWork.Posts.Commit();
                }

                return StatusCode(StatusCodes.Status201Created, new { Id = PostId });
			}
			else
			{
				return BadRequest(ModelState);
			}

		}

		[Authorize]
		[HttpPut("UpdatePost")]
		public async Task<IActionResult> UpdatePost([FromBody] PostUpdate model)
		{
			var postEntity = await _unitOfWork.Posts.GetPostById(model.Id, model.LanguageId);

			DateTime dtstartDate = new();
			DateTime? dtEndDate = new();
			DateTime? dteventDate = new();
			if (!string.IsNullOrEmpty(model.StartDateStr))
			{
				try
				{
					dtstartDate = _unitOfWork.BaseConfig.StringToDateTime(model.StartDateStr);
				}
				catch (Exception)
				{
					ModelState.AddModelError("StartDate", "Formati dates nuk eshte ne rregull");
				}
			}
			if (!string.IsNullOrEmpty(model.EndDateStr))
			{
				try
				{
					dtEndDate = _unitOfWork.BaseConfig.StringToDateTime(model.EndDateStr);
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
			if (!string.IsNullOrEmpty(model.EventDateStr))
			{
				try
				{
					dteventDate = _unitOfWork.BaseConfig.StringToDateTime(model.EventDateStr);
				}
				catch (Exception)
				{
				}
			}
			else
			{
				dteventDate = null;
			}

			if (postEntity == null)
			{
				//Krijon post te ri ne gjuhen e caktuar pasi qe nuk ekziston
				postEntity = _mapper.Map<Post>(model);
				postEntity.StartDate = dtstartDate;
				postEntity.EndDate = dtEndDate;
				postEntity.EventDate = dteventDate;
				postEntity.CreatedBy = _unitOfWork.BaseConfig.GetLoggedUserId();
				postEntity.Created = DateTime.Now;
				await _unitOfWork.Posts.Create(postEntity);
				await _unitOfWork.Posts.Commit();
			}
			else
			{
				_mapper.Map(model, postEntity);
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				postEntity.StartDate = dtstartDate;
				postEntity.EndDate = dtEndDate;
				postEntity.EventDate = dteventDate;
				postEntity.ModifiedBy = userinId;
				postEntity.Modified = DateTime.Now;

				_unitOfWork.Posts.Update(postEntity);
				await _unitOfWork.Posts.Commit();
			}

			var postsInCategory = _unitOfWork.PostsInCategory.FindByCondition(t => t.PostId == model.Id && t.LanguageId == model.LanguageId, false, null).ToList();

			var categoryToRemove = postsInCategory.Where(item => !model.PostCategoryIds.Contains(item.PostCategoryId));

			foreach (var item in categoryToRemove) {
                _unitOfWork.PostsInCategory.Delete(item);
                await _unitOfWork.PostsInCategory.Commit();
            }

			foreach (var catId in model.PostCategoryIds) {
                if (!postsInCategory.Any(item => item.PostCategoryId == catId))
                {
                    var category = await _unitOfWork.PostCategories.GetPostCaregoriesById(catId, model.LanguageId);
                    if (category != null)
                    {
                        var postsInCategoryEntity = new PostsInCategory()
                        {
                            PostCategoryId = catId,
                            PostId = model.Id,
                            LanguageId = model.LanguageId,
                            CreatedBy = _unitOfWork.BaseConfig.GetLoggedUserId(),
                            Created = DateTime.Now,
                        };

                        await _unitOfWork.PostsInCategory.Create(postsInCategoryEntity);
                        await _unitOfWork.PostsInCategory.Commit();
                    }
                }
            }


			var postsInTags = _unitOfWork.PostsInTags.FindByCondition(t => t.PostId == model.Id && t.LanguageId == model.LanguageId, false, null).ToList();

            var tagsToRemove = postsInTags.Where(item => !model.PostTagsIds.Contains(item.PostTagId)).ToList();

            foreach (var item in tagsToRemove)
            {
                _unitOfWork.PostsInTags.Delete(item);
                await _unitOfWork.PostsInTags.Commit();
            }

			if (model.PostTagsIds != null)
			{
				foreach (var tagId in model.PostTagsIds)
				{
					if (!postsInTags.Any(item => item.PostTagId == tagId))
					{

						var tags = await _unitOfWork.Tags.GetTagsById(tagId, model.LanguageId);
						if (tags != null)
						{
							var postsInTagsEntity = new PostsInTag()
							{
								PostTagId = tagId,
								LanguageId = model.LanguageId,
								PostId = model.Id,
								CreatedBy = _unitOfWork.BaseConfig.GetLoggedUserId(),
								Created = DateTime.Now,
							};

							await _unitOfWork.PostsInTags.Create(postsInTagsEntity);
							await _unitOfWork.Posts.Commit();
						}
					}
				}
			}

            var menuToReturn = _mapper.Map<PostUpdate>(postEntity);

			return Ok(menuToReturn);
		}


		[Authorize]
		[HttpDelete("DeletePost")]

		public async Task<IActionResult> DeletePost(int Id, int webLangId, bool WebMultiLang = true)
		{
			var post = await _unitOfWork.Posts.GetPostById(Id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (post == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multilinkEntity = await _unitOfWork.Posts.GetPostById(Id, item.Id);
					if (multilinkEntity != null)
					{
						multilinkEntity.Deleted = true;
						multilinkEntity.Modified = DateTime.Now;
						multilinkEntity.ModifiedBy = userId;
						_unitOfWork.Posts.Update(multilinkEntity);
						await _unitOfWork.Posts.Commit();
					}
				}
			}
			else
			{
				post.Deleted = true;
				post.Modified = DateTime.Now;
				post.ModifiedBy = userId;
				_unitOfWork.Posts.Update(post);
				await _unitOfWork.Posts.Commit();
			}

			return Ok();
		}

		[Authorize]
		[HttpPut("ChangePostStatus")]
		public async Task<IActionResult> ChangePostStatus(int postId, bool status)
		{
			var langList = await _unitOfWork.BaseConfig.GetLangList();

			foreach (var item in langList)
			{
				var postEntity = await _unitOfWork.Posts.GetPostById(postId, item.Id);
				if (postEntity != null)
				{
					postEntity.Published = status;
					_unitOfWork.Posts.Update(postEntity);
					await _unitOfWork.Posts.Commit();
				}
			}
			return StatusCode(StatusCodes.Status200OK);
		}



		#endregion

		#region PostCategory


		[Authorize]
		[HttpGet]
		[Route("GetPostCategories")]
		public async Task<IActionResult> GetPostCategories(int? LayoutId, int langId)
		{
			string[] includes = { "Layout", "Page" };
            
			var postCategoryQuery = PredicateBuilder.False<PostCategory>();
            if (LayoutId.HasValue && LayoutId > 0)
            {
                postCategoryQuery = postCategoryQuery.Or(x => x.LayoutId == LayoutId);
            }
            else
            {
                var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
                var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });
                foreach (var id in layouts.Select(x => x.Id))
                {
                    postCategoryQuery = postCategoryQuery.Or(t => t.LayoutId == id);
                }
            }
			postCategoryQuery = postCategoryQuery.And(x => x.Active == true);
			postCategoryQuery = postCategoryQuery.And(x => x.LanguageId == langId);
			var postCategoryList = await _unitOfWork.PostCategories.FindByCondition(postCategoryQuery, false, includes).OrderBy(x => x.LayoutId).ThenBy(x => x.Id).ToListAsync();

            var postCategoryDto = _mapper.Map<IEnumerable<PostCategoryListDto>?>(postCategoryList);
			return Ok(postCategoryDto);
		}

		[Authorize]
		[HttpPost]
		[Route("GetPostCategoriesAsync")]
		public async Task<IActionResult> GetPostCategoriesAsync([FromBody] PostCategoryFilterParameters parameter)
		{
			//var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			//var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
			//var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, new[] { "LayoutRoles" });

			var postCategories = await _unitOfWork.PostCategories.GetPostCategoriesAsync(parameter); //, layouts.Select(t=>t.Id).ToList()
			var postCategoriesDto = _mapper.Map<IEnumerable<PostCategoryListDto>?>(postCategories);

			return Ok(new
			{
				data = postCategoriesDto,
				total = postCategories.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetPostCaregoriesById")]
		public async Task<IActionResult> GetPostCaregoriesById(int id, int webLangId)
		{
			var data = await _unitOfWork.PostCategories.GetPostCaregoriesById(id, webLangId);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<PostCategoryDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreatePostCategory")]
		public async Task<IActionResult> CreatePostCategory([FromBody] CreatePostCategoryDto model)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				int postCategoryId = _unitOfWork.PostCategories.GetMaxPK(i => i.Id);
				var langList = await _unitOfWork.BaseConfig.GetLangList();
				model.Id = postCategoryId;
				if (model.WebMultiLang)
				{
					foreach (var item in langList)
					{
						var multiPostCategoryEntity = _mapper.Map<PostCategory>(model);
						multiPostCategoryEntity.LanguageId = item.Id;
						multiPostCategoryEntity.CreatedBy = userinId;
						multiPostCategoryEntity.Created = DateTime.Now;
						await _unitOfWork.PostCategories.Create(multiPostCategoryEntity);

						await _unitOfWork.PostCategories.Commit();
					}

					return StatusCode(StatusCodes.Status201Created);
				}

				var PostCategoryEntity = _mapper.Map<PostCategory>(model);
				PostCategoryEntity.CreatedBy = userinId;
				PostCategoryEntity.Created = DateTime.Now;
				await _unitOfWork.PostCategories.Create(PostCategoryEntity);

				await _unitOfWork.PostCategories.Commit();

				return StatusCode(StatusCodes.Status201Created, PostCategoryEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}


		[Authorize]
		[HttpPut("UpdatePostCategory")]
		public async Task<IActionResult> UpdatePostCategory([FromBody] CreatePostCategoryDto model)
		{

			var postCategoryEntity = await _unitOfWork.PostCategories.GetPostCaregoriesById(model.Id, model.LanguageId);

			if (postCategoryEntity == null)
			{
				return NotFound();
			}

			_mapper.Map(model, postCategoryEntity);
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
			postCategoryEntity.ModifiedBy = userinId;
			postCategoryEntity.Modified = DateTime.Now;

			_unitOfWork.PostCategories.Update(postCategoryEntity);
			await _unitOfWork.PostCategories.Commit();

			var menuToReturn = _mapper.Map<PostCategoryDto>(postCategoryEntity);
			return Ok(menuToReturn);
		}

		[Authorize]
		[HttpDelete("DeletePostCategory")]
		public async Task<IActionResult> DeletePostCategory(int id, int webLangId = 1, bool WebMultiLang = false)
		{
			var postCategoryId = await _unitOfWork.PostCategories.GetPostCaregoriesById(id, webLangId);

			if (postCategoryId == null)
			{
				return NotFound();
			}

			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multiPostCategoryEntity = await _unitOfWork.PostCategories.GetPostCaregoriesById(id, item.Id);
					if (multiPostCategoryEntity != null)
					{
						_unitOfWork.PostCategories.Delete(multiPostCategoryEntity);
						await _unitOfWork.PostCategories.Commit();
					}
				}

				return Ok();
			}

			_unitOfWork.PostCategories.Delete(postCategoryId);

			await _unitOfWork.PostCategories.Commit();

			return Ok();
		}

		#endregion

		#region PostMedia

		[Authorize]
		[HttpPost]
		[Route("AddMediaCollectionInPost")]
		public async Task<ActionResult> AddMediaCollectionInPost(AddMediaCollectionInPost model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var langList = await _unitOfWork.BaseConfig.GetLangList();
					string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
					var result = await _unitOfWork.Posts.AddMediaCollectionInPost(model, langList, userinId);
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
		[Route("RemoveMediaCollectionFromPost")]
		public async Task<ActionResult> RemoveMediaCollectionFromPost([FromQuery] string MediaIds, bool WebMultiLang, int webLangId, int postId)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var langList = await _unitOfWork.BaseConfig.GetLangList();
					var numbers = MediaIds.Split(',')?.Select(Int32.Parse)?.ToList();
					if (numbers.Count > 0)
					{
						var result = await _unitOfWork.Posts.RemoveMediaCollectionFromPost(numbers, langList, WebMultiLang, webLangId, postId);
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
		[Route("GetPostMedia")]
		public async Task<ActionResult> GetPostMedia(int postId, bool isSlider, int webLangId = 1)
		{
			return Ok(await _unitOfWork.Posts.GetPostMedia(postId, webLangId, isSlider));
		}


		[Authorize]
		[HttpPost]
		[Route("UpdatePostMediaOrder")]
		public async Task<ActionResult> UpdatePostMediaOrder(List<UpdatePostMediaOrderDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var postMedia = await _unitOfWork.PostMedia.GetMediaById(item.PostId, item.MediaId, item.LanguageId);
					if (postMedia != null)
					{
						postMedia.OrderNo = item.OrderNo;
						_unitOfWork.PostMedia.Update(postMedia);
					}
				}
				await _unitOfWork.PostMedia.Commit();

				return Ok();
			}

			return NotFound();
		}



        [Authorize]
        [HttpPost]
        [Route("CreateGalleryFromMediaInPost")]
        public async Task<IActionResult> CreateGalleryFromMediaInPost(int postId)
        {
            if (ModelState.IsValid)
            {
				string[] includes = { "PostMedia" };
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var existingPost = await _unitOfWork.Posts.FindByCondition(x => x.Id == postId, false, includes).ToListAsync();
                if (existingPost == null)
                {
                    return NotFound();
                }
				var _postinCategoryIds = await _unitOfWork.PostsInCategory.FindByCondition(x => x.PostId == postId, false, null).Select(t => t.PostCategoryId).Distinct().ToListAsync();
                
				var postCategoryQuery = PredicateBuilder.False<PostCategory>();

				foreach (var id in _postinCategoryIds)
				{
					postCategoryQuery = postCategoryQuery.Or(t => t.Id == id);
				}

                var _layoutIds = await _unitOfWork.PostCategories.FindByCondition(postCategoryQuery, false, null).Select(t => t.LayoutId).Distinct().ToListAsync();

                foreach (var layout in _layoutIds)
				{
                    int GaleryHeaderId = _unitOfWork.GaleryHeaders.GetMaxPK(a => a.Id);
                    foreach (var item in existingPost)
					{
						var galleryHeader = new GaleryHeader
						{
							Id =  GaleryHeaderId,
							Title = item.Title,
							LanguageId = item.LanguageId,
							LayoutId = layout,
							CategoryId = 1,
							OrderNo = 0,
							IsDeleted = false,
							ShfaqNeHome = true,
							CreatedBy = userinId,
							Created = DateTime.Now
						};

						await _unitOfWork.GaleryHeaders.Create(galleryHeader);
						await _unitOfWork.GaleryHeaders.Commit();

						foreach (var media in item.PostMedia)
						{
                            int galerydetailId = _unitOfWork.GaleryDetails.GetMaxPK(i => i.Id);
                            var galleryDetail = new GaleryDetail
							{
								Id = galerydetailId,
								HeaderId = galleryHeader.Id,
								LanguageId = media.LanguageId,
								MediaId = media.MediaId,
								OrderNo = media.OrderNo,
								CreatedBy = userinId,
								Created = DateTime.Now
							};

							await _unitOfWork.GaleryDetails.Create(galleryDetail);
							await _unitOfWork.GaleryDetails.Commit();
						}
					}
				}

                return StatusCode(StatusCodes.Status201Created);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        #endregion

        #region Trash

        [Authorize]
		[HttpPost]
		[Route("GetPostsTrashAsync")]
		public async Task<IActionResult> GetPostsTrashAsync([FromBody] PostFilterParameters parameter)
		{

			List<int>? postIds = null;
			string[] includes = { "Layout", "Page" };

			var postCategoryList = await _unitOfWork.PostCategories.FindAll(false, includes)
					.Where(t => t.LanguageId == parameter.webLangId && t.Id == (parameter.PostCategoryId > 0 ? parameter.PostCategoryId : t.Id)
					 && t.LayoutId == parameter.LayoutId).ToListAsync();


            var postInCategoryQuery = PredicateBuilder.False<PostsInCategory>();

            foreach (var id in postCategoryList.Distinct().Select(x => x.Id))
            {
                postInCategoryQuery = postInCategoryQuery.Or(t => t.PostCategoryId == id);
            }
            postInCategoryQuery = postInCategoryQuery.And(x => x.LanguageId == parameter.webLangId);

            var postInCategories = _unitOfWork.PostsInCategory.FindByCondition(postInCategoryQuery, false, null);

			postIds = postInCategories.Distinct().Select(x => x.PostId).ToList();

			var posts = await _unitOfWork.Posts.GetPostsAsync(parameter, postIds, true);
			var postsDto = _mapper.Map<IEnumerable<PostListDto>?>(posts);

			return Ok(new
			{
				data = postsDto,
				total = posts.MetaData.TotalCount
			});
		}


		[Authorize]
		[HttpDelete("DeletePostFromTrash")]
		public async Task<IActionResult> DeletePostFromTrash(int Id, int webLangId, bool WebMultiLang = true)
		{
			var post = await _unitOfWork.Posts.GetPostById(Id, webLangId);

			if (post == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var postsInCategory = _unitOfWork.PostsInCategory.FindByCondition(t => t.PostId == Id && t.LanguageId == item.Id, false, null).ToList();
					foreach (var catitem in postsInCategory)
					{
						_unitOfWork.PostsInCategory.Delete(catitem);
						await _unitOfWork.PostsInCategory.Commit();
					}
                    var postsInTags = _unitOfWork.PostsInTags.FindByCondition(t => t.PostId == Id && t.LanguageId == item.Id, false, null).ToList();
                    foreach (var catitem in postsInTags)
                    {
                        _unitOfWork.PostsInTags.Delete(catitem);
                        await _unitOfWork.PostsInTags.Commit();
                    }
                    var postsMedia = _unitOfWork.PostMedia.FindByCondition(t => t.PostId == Id && t.LanguageId == item.Id, false, null).ToList();
					foreach (var mediaitem in postsMedia)
					{
						_unitOfWork.PostMedia.Delete(mediaitem);
						await _unitOfWork.PostMedia.Commit();
					}
					var multilinkEntity = await _unitOfWork.Posts.GetPostById(Id, item.Id);
					if (multilinkEntity != null)
					{
						_unitOfWork.Posts.Delete(multilinkEntity);
						await _unitOfWork.Posts.Commit();
					}
				}
			}
			else
			{
				var postsInCategory = _unitOfWork.PostsInCategory.FindByCondition(t => t.PostId == Id && t.LanguageId == webLangId, false, null).ToList();
				foreach (var item in postsInCategory)
				{
					_unitOfWork.PostsInCategory.Delete(item);
					await _unitOfWork.PostsInCategory.Commit();
				}
                var postsInTags = _unitOfWork.PostsInTags.FindByCondition(t => t.PostId == Id && t.LanguageId == webLangId, false, null).ToList();
                foreach (var catitem in postsInTags)
                {
                    _unitOfWork.PostsInTags.Delete(catitem);
                    await _unitOfWork.PostsInTags.Commit();
                }
                var postsMedia = _unitOfWork.PostMedia.FindByCondition(t => t.PostId == Id && t.LanguageId == webLangId, false, null).ToList();
				foreach (var item in postsMedia)
				{
					_unitOfWork.PostMedia.Delete(item);
					await _unitOfWork.PostMedia.Commit();
				}

				_unitOfWork.Posts.Delete(post);
				await _unitOfWork.Posts.Commit();
			}

			return Ok();
		}

		[Authorize]
		[HttpPut("RestorePostFromTrash")]
		public async Task<IActionResult> RestorePostFromTrash(int Id, int webLangId, bool WebMultiLang = true)
		{
			var post = await _unitOfWork.Posts.GetPostById(Id, webLangId);
			string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
			if (post == null)
			{
				return NotFound();
			}
			var langList = await _unitOfWork.BaseConfig.GetLangList();
			if (WebMultiLang)
			{
				foreach (var item in langList)
				{
					var multilinkEntity = await _unitOfWork.Posts.GetPostById(Id, item.Id);
					if (multilinkEntity != null)
					{
						multilinkEntity.Deleted = false;
						multilinkEntity.Modified = DateTime.Now;
						multilinkEntity.ModifiedBy = userId;
						_unitOfWork.Posts.Update(multilinkEntity);
						await _unitOfWork.Posts.Commit();
					}
				}
			}
			else
			{
				post.Deleted = false;
				post.Modified = DateTime.Now;
				post.ModifiedBy = userId;
				_unitOfWork.Posts.Update(post);
				await _unitOfWork.Posts.Commit();
			}

			return Ok();
		}
		#endregion
	}
}

