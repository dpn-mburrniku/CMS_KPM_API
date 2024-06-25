using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class LinkRepository : GenericRepository<Link>, ILinkRepository 
    {
        public LinkRepository(CmsContext cmsContext) : base(cmsContext)
        {
            
        }

        public async Task<Link?> GetLinkById(int id, int webLangId)
        {
            var link = await _cmsContext.Links.FindAsync(id, webLangId);
            return link;
        }

        public async Task<PagedList<Link>> GetLinkAsync(LinkFilterParameters parameter)
        {
            IQueryable<Link> data = _cmsContext.Links.Include(x => x.Layout).Include(x => x.Page).Include(x => x.LinkType).IgnoreAutoIncludes().AsNoTracking()
                            .FilterLinkByLanguage(parameter.webLangId)
                            .FilterLinkByLayout(parameter.LayoutId)
                            .FilterLinkByType(parameter.LinkTypeId)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Link>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        #region web
        public async Task<List<LinksModel>> GetLinks(int GjuhaId, int LinkTypeID, int PageID)
        {
            var linksLista = new List<LinksModel>();


            var page = _cmsContext.Pages.Where(t => t.Id == PageID).FirstOrDefault();
            if (page != null)
            {
                linksLista = await (from l in _cmsContext.Links.Include(t=>t.LinkType)

                                    where l.LanguageId == GjuhaId && l.Active == true
                                          && l.TypeId == LinkTypeID && l.PageId == PageID
                                    select new LinksModel
                                    {
                                        LinkuId = l.Id,
                                        Linku = l.Url,
                                        LinkuHape = l.LinkTarget,
                                        LinkuAktiv = l.Active,
                                        LinkuLlojiId = l.TypeId,
                                        LinkuPershkrimi = l.LinkName,
                                        NrOrder = l.OrderNo,
                                        LinkuLlojiPershkrimi = l.LinkType.LinkuTypeName
                                    }).OrderBy(t => t.NrOrder).ToListAsync();
            }

            if (linksLista.Count == 0 && page != null)
            {
                linksLista = await (from l in _cmsContext.Links.Include(t => t.LinkType)

                                    where l.LanguageId == GjuhaId && l.Active == true
                                          && l.TypeId == LinkTypeID
                                    && (page != null ? l.LayoutId == page.LayoutId : true)
                                    select new LinksModel
                                    {
                                        LinkuId = l.Id,
                                        Linku = l.Url,
                                        LinkuHape = l.LinkTarget,
                                        LinkuAktiv = l.Active,
                                        LinkuLlojiId = l.TypeId,
                                        LinkuPershkrimi = l.LinkName,
                                        NrOrder = l.OrderNo,
                                        LinkuLlojiPershkrimi = l.LinkType.LinkuTypeName
                                    }).OrderBy(t => t.NrOrder).ToListAsync();
            }

            if (linksLista.Count == 0)
            {
                linksLista = await (from l in _cmsContext.Links.Include(t => t.LinkType)

                                    where l.LanguageId == GjuhaId && l.Active == true
                                          && l.TypeId == LinkTypeID && l.PageId == null && l.LayoutId == null
                                    select new LinksModel
                                    {
                                        LinkuId = l.Id,
                                        Linku = l.Url,
                                        LinkuHape = l.LinkTarget,
                                        LinkuAktiv = l.Active,
                                        LinkuLlojiId = l.TypeId,
                                        LinkuPershkrimi = l.LinkName,
                                        NrOrder = l.OrderNo,
                                        LinkuLlojiPershkrimi = l.LinkType.LinkuTypeName
                                    }).OrderBy(t => t.NrOrder).ToListAsync();
            }

            return linksLista;
        }
        #endregion
    }
}
