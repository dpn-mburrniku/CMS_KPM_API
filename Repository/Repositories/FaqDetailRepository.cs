using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class FaqDetailRepository : GenericRepository<Faqdetail>, IFaqDetailRepository
    {
        public FaqDetailRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {
            
        }

        public async Task<Faqdetail?> GetFaqDetailById(int id, int webLangId)
        {
            var FaqDetail = await _cmsContext.Faqdetails.FindAsync(id, webLangId);
            return FaqDetail;
        }

        public async Task<Faqdetail> GetDetailsById(int id,int langId, int headerId)
        {
            var media = await _cmsContext.Faqdetails.Where(x => x.Id == id && x.LanguageId == langId && x.HeaderId == headerId).FirstOrDefaultAsync();

            return media;
        }

        public async Task<PagedList<Faqdetail>> GetFaqDetailAsync(FaqDetailParameters parameter)
        {
            IQueryable<Faqdetail> data = _cmsContext.Faqdetails.Include(x => x.Faqheader).IgnoreAutoIncludes().AsNoTracking()
                            .FilterFaqDetailByLanguage(parameter.webLangId)
                            .FilterFaqDetailByHeader(parameter.FaqHeaderId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Faqdetail>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }
    }
}
