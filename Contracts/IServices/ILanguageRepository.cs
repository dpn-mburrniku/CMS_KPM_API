using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<Language?> GetLanguageById(int id);
        Task<PagedList<Language>> GetLanguageAsync(LanguageParameters parameter);
        Task<List<CultureCode>?> GetCultureCode();
    }
}
