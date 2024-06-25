using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface ISearchRepository
    {
        Task<List<SearchModel>> SearchAll(string parameter, int GjuhaID, int skip, int take, DateTime formatedDateTime);
        Task<int> SearchAllCount(string parameter, int GjuhaID, DateTime formatedDateTime);  
    }
}
