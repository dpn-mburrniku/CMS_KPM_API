using Entities.Models;
using Entities.RequestFeatures;

namespace Contracts.IServices
{
    public interface IMunicipalityRepository : IGenericRepository<Municipality>
    {
        Task<Municipality?> GetMunicipalityById(int id);
        Task<PagedList<Municipality>> GetMunicipalitiesAsync(FilterParameters parameter);
    }
}