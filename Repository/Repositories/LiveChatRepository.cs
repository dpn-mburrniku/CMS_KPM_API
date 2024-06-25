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
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
    public class LiveChatRepository : GenericRepository<LiveChat>, ILiveChatRepository
    {
        public LiveChatRepository(CmsContext cmsContext) : base(cmsContext)
        {

        }

        public async Task<LiveChat?> GetLiveChatById(int id, int webLangId)
        {
            var liveChat = await _cmsContext.LiveChats.FindAsync(id, webLangId);

            return liveChat;

        }

        public async Task<PagedList<LiveChat>> GetLiveChatAsync(LiveChatFilterParameters parameter)
        {
            IQueryable<LiveChat> data = _cmsContext.LiveChats.Include(x => x.Page)
                          .Include(x => x.LiveChatNavigation)
                                .ThenInclude(x => x.Page)
                                .Include(x => x.LiveChatNavigation)
                                    .ThenInclude(x => x.Page)
                                    .Include(x => x.LiveChatNavigation)
                                        .ThenInclude(x => x.Page)
                                        .Include(x => x.LiveChatNavigation)
                          .IgnoreAutoIncludes().AsNoTracking()
                          .FilterLiveChatByLanguage(parameter.webLangId)
                          .FilterLiveChatByPage(parameter.PageId)
                          .Search(parameter.Query)
                          .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                          //.ToListAsync();

            return PagedList<LiveChat>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        #region web
        public async Task<List<LiveChatModel>> GetLiveChat(int GjuhaId, int? ParentId, string searchText)
        {
            var result = await _cmsContext.LiveChats.Where(t => t.LanguageId == GjuhaId &&
                        (string.IsNullOrEmpty(searchText) ? (ParentId == null ? t.Level == 1 : t.Id == ParentId && t.Active != false) : t.Name.Contains(searchText)))
                 .Select(t => new LiveChatModel
                 {
                     Id = t.Id,
                     GjuhaId = t.LanguageId,
                     Level = t.Level,
                     ParentId = t.ParentId,
                     Name = t.Name,
                     Description = t.Description,
                     PageId = t.PageId,
                     IsOtherSource = (int)t.PageId > 0 ? false : true,
                     Url = (int)t.PageId > 0 ?
                     _cmsContext.Pages.Include(x => x.Template).Include(x => x.Layout).Where(x => x.Id == t.PageId && x.LanguageId == GjuhaId)
                     .Select(l => l.Layout.Path + l.Template.TemplateUrl + "/" + l.Id).FirstOrDefault()
                     : t.OtherSource,
                     LinkName = t.OtherSourceName,
                     OrderNr = t.OrderNo,
                     HasChilds = _cmsContext.LiveChats.Where(x => x.ParentId == t.Id && x.LanguageId == GjuhaId).Count() > 0 ? true : false,
                     childs = _cmsContext.LiveChats
                                         .Where(x => x.ParentId == t.Id && x.LanguageId == GjuhaId &&
                                         (string.IsNullOrEmpty(searchText) ? x.Active != false : true))
                                         .Select(c => new ChildLiveChatModel { Id = c.Id, Name = c.Name, OrderNr = c.OrderNo }).OrderBy(t => t.OrderNr).ToList(),
                 }).OrderBy(t => t.OrderNr).ToListAsync();

            return result;
        }
        #endregion

    }
}

