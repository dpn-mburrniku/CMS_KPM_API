using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.DataTransferObjects;
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
    public class PostMediaRepository : GenericRepository<PostMedium>, IPostMediaRepository
    {
        public PostMediaRepository(CmsContext cmsContext) : base(cmsContext)
        {
            
        }

        public async Task<PostMedium> GetMediaById(int postId, int id, int langId)
        {
            var media = await _cmsContext.PostMedia.Where(x => x.MediaId == id && x.PostId == postId && x.LanguageId == langId).FirstOrDefaultAsync();

            return media;
        }
    }
}
