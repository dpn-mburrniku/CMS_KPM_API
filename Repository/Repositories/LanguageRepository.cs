using Contracts.IServices;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

namespace Repository.Repositories
{
    internal class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<Language?> GetLanguageById(int id)
        {
            var language = await _cmsContext.Languages.FindAsync(id);

            return language;

        }

        public async Task<PagedList<Language>> GetLanguageAsync(LanguageParameters parameter)
        {
            IQueryable<Language> data = _cmsContext.Languages.Include(x => x.CultureCode).IgnoreAutoIncludes().AsNoTracking()
                            .Search(parameter.Query, parameter.webLangId)
                            .FilterLanguages(parameter.CultureCodeId)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);


            return PagedList<Language>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public async Task<List<CultureCode>?> GetCultureCode()
        {
            var language = await _cmsContext.CultureCodes.ToListAsync();

            return language;
        }
    }
}
