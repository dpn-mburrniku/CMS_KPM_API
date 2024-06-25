using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ResourceTranslationRepository : GenericRepository<ResourceTranslationString>, IResourceTranslationRepository
    {
        public ResourceTranslationRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<PagedList<ResourceTranslationString>> GetResourceTranslationAsync(ResourceTranslationStringFilterParameters parameter)
        {
            IQueryable<ResourceTranslationString> data = _cmsContext.ResourceTranslationStrings.Include(x => x.Type).IgnoreAutoIncludes().AsNoTracking()
                                                  .FilterResourceTranslationByLanguage(parameter.webLangId)
                                                  .FilterResourceTranslationByType(parameter.ResourceTranslationTypeId)
                                                  .Search(parameter.Query)
                                                  .Sort(parameter.Sort.key + " " + parameter.Sort.order);
             
            return PagedList<ResourceTranslationString>.ToPagedList(data, parameter.PageIndex, parameter.PageSize);
        }

        public async Task<ResourceTranslationString?> GetResourceTranslationById(int id, int webLangId)
        {
            var resource = await _cmsContext.ResourceTranslationStrings.FindAsync(id, webLangId);

                return resource;
        }

        public async Task<List<ResourceTranslationType>?> GetResourceTranslationType()
        {
            var translationType = await _cmsContext.ResourceTranslationTypes.ToListAsync();

            return translationType;
        }

        public async Task<List<ResourceTranslationType>?> GetResourceTranslationTypeWithIncludes(int LangId)
        {
            var translationType = await _cmsContext.ResourceTranslationTypes.Include(t => t.ResourceTranslationStrings.Where(x=>x.LanguageId == LangId)).AsNoTracking().ToListAsync();

            return translationType;
        }

        public async Task<List<ResourceTranslationString>?> GetResourceTranslationByLang(int langId)
        {
            var resources = await _cmsContext.ResourceTranslationStrings.Where(e => e.LanguageId == langId).ToListAsync();

            return resources;
        }

        public bool ResourceExist(int webLangId, int id)
        {
            return _cmsContext.ResourceTranslationStrings.Any(e => e.Id == id && e.LanguageId == webLangId);
        }


    }
}
