using Entities.Models; using CMS.API;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IPostMediaRepository : IGenericRepository<PostMedium>
    {
        Task<PostMedium> GetMediaById(int postId, int id, int langId);
    }
}
