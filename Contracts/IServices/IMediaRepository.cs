using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IMediaRepository : IGenericRepository<Medium>
    {
        Task<PagedList<Medium>> GetMediaAsync(MediaFilterParameters parameter);
        Task<List<string>> GetMediaEx();
        Task<MediaEx> GetMediaExCategory(string mediaEx);
    }


}
