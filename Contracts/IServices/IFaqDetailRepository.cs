using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IFaqDetailRepository: IGenericRepository<Faqdetail>
    {
        Task<Faqdetail?> GetFaqDetailById(int id, int webLangId);
        Task<PagedList<Faqdetail>> GetFaqDetailAsync(FaqDetailParameters parameter);

        Task<Faqdetail> GetDetailsById(int id, int langId, int headerId);
    }
}
