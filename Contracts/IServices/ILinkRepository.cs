using Entities.Models; 
using Entities.RequestFeatures;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface ILinkRepository: IGenericRepository<Link>
    {
        Task<Link?> GetLinkById(int id, int webLangId);
        Task<PagedList<Link>> GetLinkAsync(LinkFilterParameters parameter);


        //web
        Task<List<LinksModel>> GetLinks(int GjuhaId, int LinkTypeID, int PageID);
    }
}
