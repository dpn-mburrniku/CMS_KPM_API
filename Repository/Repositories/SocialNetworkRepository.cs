using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class SocialNetworkRepository : GenericRepository<SocialNetwork>, ISocialNetworkRepository
    {
        public SocialNetworkRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {

        }
        public async Task<SocialNetwork?> GetSocialNetworkById(int id, int webLangId)
        {
            var socialnetwork = await _cmsContext.SocialNetworks.FindAsync(id, webLangId);

            return socialnetwork;
        }

        public async Task<PagedList<SocialNetwork>> GetSocialNetworkAsync(SocialNetworkFilterParameters parameter )
        {
            IQueryable<SocialNetwork> data = _cmsContext.SocialNetworks.Include(x => x.Layout).Include(x => x.ComponentLocation).IgnoreAutoIncludes().AsNoTracking()
                            .FilterSocialNetworkByLanguage(parameter.webLangId)
                            .FilterSocialNetworkByLayout(parameter.LayoutId)
                            .FilterSocialNetworkByLocation(parameter.ComponentLocationId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<SocialNetwork>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        #region web
        public async Task<List<SocialNetworksModel>> GetSocialNetworks(int GjuhaId, int? LayoutID, int? ComponentLocationId)
        {
            var result = await _cmsContext.SocialNetworks.Where(t => t.LanguageId == GjuhaId && t.LayoutId == LayoutID && t.Active != false
                                                                   && t.ComponentLocationId == ComponentLocationId)
                .Select(t => new SocialNetworksModel
                {
                    Id = t.Id,
                    LayoutId = t.LayoutId,
                    GjuhaId = t.LanguageId,
                    Emertimi = t.Name,
                    Linku = t.Link,
                    ImgPath = t.ImgPath,
                    Html = t.Html,
                    Aktiv = t.Active,
                    OrderNr = t.OrderNo
                }).OrderBy(t => t.OrderNr).ToListAsync();

            return result;
        }

        public async Task<List<ResourceTranslationType>?> GetResourceTranslationsByTypeAndLanguage()
        {
            var translationList = await _cmsContext.ResourceTranslationTypes.Include(x => x.ResourceTranslationStrings).IgnoreAutoIncludes().AsNoTracking().ToListAsync();

            return translationList;
        }
        #endregion
    }
}
