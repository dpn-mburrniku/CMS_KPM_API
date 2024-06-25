using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
	public interface IGaleryHeaderRepository : IGenericRepository<GaleryHeader>
	{
		Task<GaleryHeader?> GetGaleryHeaderById(int id, int webLangId);
		Task<PagedList<GaleryHeader>> GetGaleryHeaderAsync(GaleryFilterParameters parameter, bool isDeleted);

		//web
		Task<int> GetPageId(int CategoryId, string? Layout);
        Task<List<GaleryModel>> GetGaleries(int gjuhaID, int CategoryId, int? LayoutID, int? PageID, int skip, int take);
        Task<int> GetGaleriesCount(int gjuhaID, int CategoryId, int? LayoutID, int? PageID);

    }

}
