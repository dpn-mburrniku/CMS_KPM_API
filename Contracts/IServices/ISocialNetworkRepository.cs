using Entities.Models; using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface ISocialNetworkRepository : IGenericRepository<SocialNetwork>
    {
        Task<SocialNetwork?> GetSocialNetworkById(int id, int webLangId);
        Task<PagedList<SocialNetwork>> GetSocialNetworkAsync(SocialNetworkFilterParameters parameter);

        //web
        Task<List<SocialNetworksModel>> GetSocialNetworks(int GjuhaId, int? LayoutID, int? LlojiID);
    }  
}
