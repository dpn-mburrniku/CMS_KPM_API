using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class LinkTypeRepository : GenericRepository<LinkType>, ILinkTypeRepository
    {
        public LinkTypeRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {

        }

        public async Task<LinkType?> GetLinksTypesById(int id, int webLangId)
        {
            var link = await _cmsContext.LinkTypes.FindAsync(id, webLangId);
            return link;
        }

        public async Task<PagedList<LinkType>> GetLinksTypesAsync(LinkTypeFilterParameters parameter)
        {
            IQueryable<LinkType> data = _cmsContext.LinkTypes.Include(x => x.ComponentLocation).IgnoreAutoIncludes().AsNoTracking()
                            .FilterLinkTypesByLanguage(parameter.webLangId)
                            .FilterLinkTypesByLocation(parameter.ComponentLocationId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<LinkType>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

    }
}
