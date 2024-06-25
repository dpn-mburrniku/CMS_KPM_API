using Entities.Models;
using CMS.API;
using CMS.API;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
	public interface IPageRepository : IGenericRepository<Page>
	{
		Task<Page> GetPageById(int id, int webLangId);
		Task<PagedList<Page>> GetPagesAsync(PageFilterParameters parameter);
		Task<bool> AddMediaCollectionInPage(AddMediaCollectionInPage model, List<Language> langList, string UserId);
		Task<bool> RemoveMediaCollectionFromPage(List<int> MediaIds, List<Language> langList, bool WebMultiLang, int webLangId, int pageId);
		Task<List<GetPageMediaDto>> GetPageMedia(int pageId, int webLangId, bool isSlider);
		Task<PagedList<Page>> GetSubPagesAsync(PageFilterParameters parameter);
		Task<PagedList<Page>> GetSubPagesTrashAsync(SubPageFilterParameters parameter);
		// web
		Task<List<PageModel>> GetBasicPage(int PageID, int GjuhaID, DateTime formatedDateTime);
		Task<object> GetPageMedia(int PageID, int GjuhaID, int skip, int take, int Viti, string searchText, DateTime formatedDateTime);
		Task<object> GetPageSlider(int PageID, int GjuhaID);
		Task<int> GetPageMediaCount(int PageId, int GjuhaId, int Viti, string searchText, DateTime formatedDateTime);
		Task<List<VitetModel>> GetVitet(int PageId, int GjuhaId, DateTime formatedDateTime);
		Task<List<PageModel>> GetSubPages(int PageId, int GjuhaId, DateTime formatedDateTime, int? top);
		public int? GetPageIdFromPostimiGategoriaID(int PostimiKategoriaID, int GjuhaId);
	}
}
