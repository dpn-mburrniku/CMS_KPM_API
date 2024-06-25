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
    public class PostsInTagsRepository : GenericRepository<PostsInTag>, ITagInPostRepository
    {
        public PostsInTagsRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {

        }
    }
}
