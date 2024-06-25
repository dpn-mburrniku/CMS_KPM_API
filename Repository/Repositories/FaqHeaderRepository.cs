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
    public class FaqHeaderRepository : GenericRepository<Faqheader>, IFaqHeaderRepository
    {
        public FaqHeaderRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {
            
        }

        public async Task<Faqheader?> GetFaqHeaderById(int id, int webLangId)
        {
            var FaqHeader = await _cmsContext.Faqheaders.FindAsync(id, webLangId);
            return FaqHeader;
        }

        public async Task<PagedList<Faqheader>> GetFaqHeaderAsync(FaqHeaderParameters parameter)
        {
            IQueryable<Faqheader> data = _cmsContext.Faqheaders.Include(x => x.Layout).Include(x => x.Page).IgnoreAutoIncludes().AsNoTracking()
                            .FilterFaqHeaderByLanguage(parameter.webLangId)
                            .FilterFaqHeaderByLayout(parameter.LayoutId)
                            .FilterFaqHeaderByPage(parameter.PageId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Faqheader>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }
    }
}
