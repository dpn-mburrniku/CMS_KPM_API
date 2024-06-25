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
	public interface IGaleryDetailRepository : IGenericRepository<GaleryDetail> 
	{
		Task<GaleryDetail?> GetGaleryDetailById(int id, int webLangId);		
		Task<PagedList<GaleryDetail>> GetGaleryDetailAsync(GaleryDetailParameters parameter);

		//web
        Task<List<GaleryDetailsModel>> GetGaleryDetails(int MediaGaleriaID, int LanguageId);

        Task<GaleryDetail> GetMediaById(int id, int langId, int headerId);
    }
}
