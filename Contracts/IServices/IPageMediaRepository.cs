

using Entities.DataTransferObjects;
using Entities.Models;

namespace Contracts.IServices
{
    public interface IPageMediaRepository : IGenericRepository<PageMedium>
    {
        Task<PageMedium> GetMediaById(int id, int langId, int pageId);

    }
}
