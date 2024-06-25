using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Repository.Extensions;

namespace Repository.Repositories
{
    public class MunicipalityRepository : GenericRepository<Municipality>, IMunicipalityRepository
    {
        public MunicipalityRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<Municipality?> GetMunicipalityById(int id)
        {
            var municipality = await _cmsContext.Municipalities.FindAsync(id);

            return municipality;

        }

        public async Task<PagedList<Municipality>> GetMunicipalitiesAsync(FilterParameters parameter)
        {
            IQueryable<Municipality> data = _cmsContext.Municipalities
                            .Search(parameter.Query, parameter.webLangId)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
            

            return PagedList<Municipality>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

    }
}