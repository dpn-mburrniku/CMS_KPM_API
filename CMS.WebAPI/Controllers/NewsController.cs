using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CMS.WebAPI.Controllers
{
	[EnableCors]
	[ApiController]
	[Route("api/[controller]")]
	public class NewsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }
		public NewsController(IUnitOfWork unitOfWorkRepository, IMapper mapper)
		{
			_unitOfWork = unitOfWorkRepository;
			_mapper = mapper;
		}

		[HttpGet("GetNews")]
		public async Task<IActionResult> GetNews(string? PostimiKategoriaID, int PageId = 0, string Gjuha = "sq", int skip = 0, int take = 10, int TitulliLength = 100, int PermbajtjaLength = 100,
				DateTime? date = null, string? SearchText = "", DateTime? DateFrom = null, DateTime? DateTo = null)
		{
			int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
			DateTime data = _unitOfWork.BaseConfig.GetDateTime();
			List<int> _postimiKategoriaID = new List<int>();

			if (PostimiKategoriaID == null)
			{
				_postimiKategoriaID.Add(0);
			}
			else if (PostimiKategoriaID == "0" || PostimiKategoriaID == "")
			{
				_postimiKategoriaID = _unitOfWork.Posts.GetNewsCategories(0, gjuhaID, false).Result.Select(x => x.PostimetKategoriaId).Cast<int>().ToList();
			}
			else
			{
				_postimiKategoriaID = PostimiKategoriaID?.Split(',')?.Select(Int32.Parse)?.ToList();
			}

			if (PageId == 0)
			{
				PageId = (int)_unitOfWork.Pages.GetPageIdFromPostimiGategoriaID(_postimiKategoriaID.FirstOrDefault(), gjuhaID);
			}
			var page = await _unitOfWork.Pages.GetBasicPage(PageId, gjuhaID, data);

			DateTime? filterDate = null;

			if (string.IsNullOrEmpty(SearchText) && DateFrom == null && DateTo == null)
			{
				filterDate = date;
			}

			var lajmetLista = await _unitOfWork.Posts.GetNews(gjuhaID, _postimiKategoriaID, skip, take, TitulliLength, PermbajtjaLength, filterDate, SearchText, DateFrom, DateTo, data);

			int totalRowsCount = await _unitOfWork.Posts.GetNewsCount(gjuhaID, _postimiKategoriaID, filterDate, SearchText, DateFrom, DateTo, data);

			int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);


			return Ok(new { page, lajmetLista, totalRowsCount, totalPages });
		}

		[HttpGet("GetNewsWithCategoryFilter")]
		public async Task<IActionResult> GetNewsWithCategoryFilter(string? PostimiKategoriaID, int PageId, string Gjuha = "sq", int skip = 0, int take = 10, int TitulliLength = 100, int PermbajtjaLength = 100, DateTime? date = null,
				string? SearchText = "", DateTime? DateFrom = null, DateTime? DateTo = null)
		{
			int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
			DateTime data = _unitOfWork.BaseConfig.GetDateTime();

			List<int> _postimiKategoriaID = new List<int>();

			if (PostimiKategoriaID == null)
			{
				_postimiKategoriaID.Add(0);
			}
			else if (PostimiKategoriaID == "0" || PostimiKategoriaID == "")
			{
				_postimiKategoriaID = _unitOfWork.Posts.GetNewsCategories(PageId, gjuhaID, true).Result.Select(x => x.PostimetKategoriaId).Cast<int>().ToList();
			}
			else
			{
				_postimiKategoriaID = PostimiKategoriaID?.Split(',')?.Select(Int32.Parse)?.ToList();
			}

			var page = await _unitOfWork.Pages.GetBasicPage(PageId, gjuhaID, data);

			DateTime? filterDate = null;

			if (string.IsNullOrEmpty(SearchText) && DateFrom == null && DateTo == null)
			{
				filterDate = date;
			}

			var lajmetLista = await _unitOfWork.Posts.GetNews(gjuhaID, _postimiKategoriaID, skip, take, TitulliLength, PermbajtjaLength, filterDate, SearchText, DateFrom, DateTo, data);

			int totalRowsCount = await _unitOfWork.Posts.GetNewsCount(gjuhaID, _postimiKategoriaID, filterDate, SearchText, DateFrom, DateTo, data);

			int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);


			return Ok(new { page, lajmetLista, totalRowsCount, totalPages });
		}

		[HttpGet("GetNewsDetail")]
		public async Task<IActionResult> GetNewsDetail(int PostimiID, string Gjuha = "sq")
		{
			int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

			var postimi = await _unitOfWork.Posts.GetNewsDetails(gjuhaID, PostimiID);

			var postimiCategory = await _unitOfWork.Posts.FindAll(false, null).Where(x => x.Id == PostimiID && x.LanguageId == gjuhaID).Include(x => x.PostsInCategories).ThenInclude(x => x.PostCategory).SelectMany(x => x.PostsInCategories.Select(t => t.PostCategory.Title)).ToListAsync();

			postimi.PostimiKategoria = postimiCategory.FirstOrDefault();

			var mediaPostimi = await _unitOfWork.Posts.GetNewsDetailsMedia(gjuhaID, PostimiID, true);

            var mediaPostimiDocuments = await _unitOfWork.Posts.GetNewsDetailsMedia(gjuhaID, PostimiID, false);

            return Ok(new { postimi, mediaPostimi, mediaPostimiDocuments });
		}

		[HttpGet("GetNewsCategories")]
		public async Task<IActionResult> GetNewsCategories(int PageId, string Gjuha = "sq")
		{
			int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

			var lista = await _unitOfWork.Posts.GetNewsCategories(PageId, gjuhaID, false);

			return Ok(lista);
		}

		[HttpGet("GetCheckNewsForThisDate")]
		public async Task<IActionResult> GetCheckNewsForThisDate(string Gjuha = "sq", DateTime? date = null)
		{
			int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

			int countNews = await _unitOfWork.Posts.GetCheckNewsForThisDate(gjuhaID, date);

			return Ok(new { countNews });
		}
		//[HttpGet("GetNews")]
		//public async Task<IActionResult> GetNews(string? PostimiKategoriaID, int PageId = 0, string Gjuha = "sq", int skip = 0, int take = 10, int TitulliLength = 100, int PermbajtjaLength = 100,
		//    DateTime? date = null, string? SearchText = "", DateTime? DateFrom = null, DateTime? DateTo = null)
		//{
		//    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
		//    DateTime data = _unitOfWork.BaseConfig.GetDateTime();
		//    List<int> _postimiKategoriaID = new List<int>();

		//    if (PostimiKategoriaID == null)
		//    {
		//        _postimiKategoriaID.Add(0);
		//    }
		//    else if (PostimiKategoriaID == "0" || PostimiKategoriaID == "")
		//    {
		//        _postimiKategoriaID = await _unitOfWork.PostCategories.FindByCondition(t => t.LanguageId == gjuhaID && t.ShowInFilters == true, false, null).Select(x => x.Id).Cast<int>().ToListAsync();
		//    }
		//    else
		//    {
		//        _postimiKategoriaID = PostimiKategoriaID?.Split(',')?.Select(Int32.Parse)?.ToList();
		//    }

		//    if (PageId == 0)
		//    {
		//        var postcat = await _unitOfWork.PostCategories.GetPostCaregoriesById(_postimiKategoriaID.FirstOrDefault(), gjuhaID);
		//        PageId = (int)postcat.PageId;
		//    }
		//    string[] includes = { "Template", "Layout", "Media" };
		//    var page = await _unitOfWork.Pages.FindByCondition(x=>x.Id == PageId && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

		//    var mappedPage = _mapper.Map<Page, BasicPageModel>(page);

		//    DateTime? filterDate = null;

		//    if (string.IsNullOrEmpty(SearchText) && DateFrom == null && DateTo == null)
		//    {
		//        filterDate = date;
		//    }

		//    //var lajmetLista = await _unitOfWork.News.GetNews(gjuhaID, _postimiKategoriaID, skip, take, TitulliLength, PermbajtjaLength, filterDate, SearchText, DateFrom, DateTo, data);
		//    var posList = await _unitOfWork.PostsInCategory.FindByCondition(p => p.LanguageId == gjuhaID
		//                             &&  p.Post.Published == true && p.Post.Deleted != true
		//                             && (filterDate != null ? p.Post.StartDate >= filterDate && p.Post.StartDate < filterDate.Value.AddDays(1) : true)
		//                             && (filterDate != null ? p.Post.StartDate <= filterDate && (p.Post.EndDate != null ? p.Post.EndDate.Value >= filterDate : true) : true)
		//                             && (!string.IsNullOrEmpty(SearchText) ? p.Post.Title.Contains(SearchText) : true)
		//                             && (DateFrom != null ?
		//                                 (p.Post.StartDate.Date >= DateFrom.Value) &&
		//                                 (p.Post.EndDate != null ? p.Post.EndDate.Value.Date >= DateFrom.Value : true) : true)
		//                                 && (DateTo != null ?
		//                                 (p.Post.StartDate.Date <= DateTo.Value) : true)
		//                             , false, new[] { "Post", "Post.Media", "Post.Media.FileExNavigation", "PostCategory", "PostCategory.Page", "PostCategory.Page.Layout" })
		//        .Distinct().ToListAsync();

		//    var mappedPostList = _mapper.Map<List<PostsInCategory>, List<NewsInCategoriesModel>>(posList);

		//    foreach (var post in mappedPostList)
		//    {
		//        post.Post.Url = post.PostCategory.PageId != null && post.PostCategory.PageId > 0 ?
		//                                            post.PostCategory.Page.Layout.Path + "/NewsDetails/" + post.PostCategory.PageId + "/" + post.PostId
		//                                            : "/NewsDetails/" + _unitOfWork.Pages.FindByCondition(t => t.TemplateId == 5,false,null).FirstOrDefault().Id + "/" + post.PostId;
		//        post.Post.Title = post.Post.Title.Length > TitulliLength ? post.Post.Title.Substring(0, Math.Min(post.Post.Title.Length, TitulliLength)) + "..." : post.Post.Title;
		//        post.Post.Description = post.Post.Description.Length > PermbajtjaLength ? post.Post.Description.Substring(0, Math.Min(post.Post.Description.Length, PermbajtjaLength)) + "..." : post.Post.Description;
		//    }

		//    var lajmetLista = mappedPostList.Select(t=>t.Post).DistinctBy(p=>p.Id).OrderByDescending(t=>t.StartDate).Skip(skip).Take(take);


		//    int totalRowsCount = mappedPostList.Distinct().Count();

		//    int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);


		//    return Ok(new { page = mappedPage, lajmetLista, totalRowsCount, totalPages });
		//}

		//[HttpGet("GetNewsWithCategoryFilter")]
		//public async Task<IActionResult> GetNewsWithCategoryFilter(int PageId, string Gjuha = "sq", int skip = 0, int take = 10, int TitulliLength = 100, int PermbajtjaLength = 100, DateTime? date = null,
		//    string? SearchText = "", DateTime? DateFrom = null, DateTime? DateTo = null)
		//{
		//    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
		//    DateTime data = _unitOfWork.BaseConfig.GetDateTime();

		//    List<int> _postimiKategoriaID = await _unitOfWork.PostCategories.FindByCondition(t => t.LanguageId == gjuhaID && t.ShowInFilters == false, false, null).Select(x => x.Id).Cast<int>().ToListAsync();

		//    string[] includes = { "Template", "Layout", "Media" };
		//    var page = await _unitOfWork.Pages.FindByCondition(x => x.Id == PageId && x.LanguageId == gjuhaID, true, includes).FirstOrDefaultAsync();

		//    var mappedPage = _mapper.Map<Page, BasicPageModel>(page);

		//    DateTime? filterDate = null;

		//    if (string.IsNullOrEmpty(SearchText) && DateFrom == null && DateTo == null)
		//    {
		//        filterDate = date;
		//    }

		//    var posList = await _unitOfWork.PostsInCategory.FindByCondition(p => p.LanguageId == gjuhaID
		//                             && p.Post.Published == true && p.Post.Deleted != true
		//                             && (filterDate != null ? p.Post.StartDate >= filterDate && p.Post.StartDate < filterDate.Value.AddDays(1) : true)
		//                             && (filterDate != null ? p.Post.StartDate <= filterDate && (p.Post.EndDate != null ? p.Post.EndDate.Value >= filterDate : true) : true)
		//                             && (!string.IsNullOrEmpty(SearchText) ? p.Post.Title.Contains(SearchText) : true)
		//                             && (DateFrom != null ?
		//                                 (p.Post.StartDate.Date >= DateFrom.Value) &&
		//                                 (p.Post.EndDate != null ? p.Post.EndDate.Value.Date >= DateFrom.Value : true) : true)
		//                                 && (DateTo != null ?
		//                                 (p.Post.StartDate.Date <= DateTo.Value) : true)
		//                             , false, new[] { "Post", "Post.Media", "Post.Media.FileExNavigation", "PostCategory", "PostCategory.Page", "PostCategory.Page.Layout" })
		//        .Distinct().ToListAsync();

		//    var mappedPostList = _mapper.Map<List<PostsInCategory>, List<NewsInCategoriesModel>>(posList);

		//    foreach (var post in mappedPostList)
		//    {
		//        post.Post.Url = post.PostCategory.PageId != null && post.PostCategory.PageId > 0 ?
		//                                            post.PostCategory.Page.Layout.Path + "/NewsDetails/" + post.PostCategory.PageId + "/" + post.PostId
		//                                            : "/NewsDetails/" + _unitOfWork.Pages.FindByCondition(t => t.TemplateId == 5, false, null).FirstOrDefault().Id + "/" + post.PostId;
		//        post.Post.Title = post.Post.Title.Length > TitulliLength ? post.Post.Title.Substring(0, Math.Min(post.Post.Title.Length, TitulliLength)) + "..." : post.Post.Title;
		//        post.Post.Description = post.Post.Description.Length > PermbajtjaLength ? post.Post.Description.Substring(0, Math.Min(post.Post.Description.Length, PermbajtjaLength)) + "..." : post.Post.Description;
		//    }

		//    var lajmetLista = mappedPostList.Select(t => t.Post).DistinctBy(p => p.Id).OrderByDescending(t => t.StartDate).Skip(skip).Take(take);


		//    int totalRowsCount = mappedPostList.Distinct().Count();

		//    int totalPages = _unitOfWork.BaseConfig.GetTotalPages(totalRowsCount, take);


		//    return Ok(new { page = mappedPage, lajmetLista, totalRowsCount, totalPages });
		//}

		//[HttpGet("GetNewsDetail")]
		//public async Task<IActionResult> GetNewsDetail(int PostimiID, string Gjuha = "sq")
		//{
		//    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

		//    var post = await _unitOfWork.Posts.FindByCondition(p => p.LanguageId == gjuhaID && p.Id == PostimiID
		//                             , false, new[] { "Media", "Media.FileExNavigation", "PostMedia", "PostMedia.Media", "PostMedia.Media.FileExNavigation" })
		//        .FirstOrDefaultAsync();

		//    var postimi = _mapper.Map<Post, NewsModel>(post);

		//    return Ok(postimi);
		//}

		//[HttpGet("GetNewsCategories")]
		//public async Task<IActionResult> GetNewsCategories(string Gjuha = "sq")
		//{
		//    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

		//    var lista = await _unitOfWork.PostCategories.FindByCondition(t => t.LanguageId == gjuhaID && t.ShowInFilters == true, false, null).ToListAsync();

		//    var categories = _mapper.Map<List<PostCategory>, List<NewsCategoriesModel>>(lista);

		//    return Ok(categories);
		//}

		//[HttpGet("GetCheckNewsForThisDate")]
		//public async Task<IActionResult> GetCheckNewsForThisDate(string Gjuha = "sq", DateTime? date = null)
		//{
		//    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

		//    date = date ?? DateTime.Now;
		//    int countNews = await _unitOfWork.Posts.FindByCondition(c => c.LanguageId == gjuhaID && c.Deleted != true && c.StartDate >= date.Value.Date
		//                                                    && c.StartDate < date.Value.Date.AddDays(1), false, null).CountAsync();

		//    return Ok(new { countNews });
		//}
	}
}
