using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface ILiveChatRepository : IGenericRepository<LiveChat>
    {
        Task<LiveChat?> GetLiveChatById(int id, int webLangId);

        Task<PagedList<LiveChat>> GetLiveChatAsync(LiveChatFilterParameters parameter);

        //web
        Task<List<LiveChatModel>> GetLiveChat(int GjuhaId, int? ParentId, string searchtext);
    }
}
