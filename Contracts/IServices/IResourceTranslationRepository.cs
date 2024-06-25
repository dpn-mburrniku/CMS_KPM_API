using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IResourceTranslationRepository : IGenericRepository<ResourceTranslationString>
    {
        Task<ResourceTranslationString?> GetResourceTranslationById(int id, int webLangId);
        Task<PagedList<ResourceTranslationString>> GetResourceTranslationAsync(ResourceTranslationStringFilterParameters parameter);
        //Task<List<ResourceTranslationString>> GetResourceTranslationByLang(int id);
        Task<List<ResourceTranslationType>?> GetResourceTranslationType();
        Task<List<ResourceTranslationType>?> GetResourceTranslationTypeWithIncludes(int LangId);
        Task<List<ResourceTranslationString>?> GetResourceTranslationByLang(int langId);

        bool ResourceExist(int webLandId, int id);
    }
}
