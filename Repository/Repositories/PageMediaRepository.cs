
using Contracts.IServices;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.Repositories
{
    public class PageMediaRepository : GenericRepository<PageMedium>, IPageMediaRepository
    {
        public PageMediaRepository(CmsContext cmsContext) : base(cmsContext)
        {
            
        }

        public async Task<PageMedium> GetMediaById(int id, int langId, int pageId)
        {
            var media = await _cmsContext.PageMedia.Include(x => x.Media).Where(x => x.MediaId == id && x.LanguageId == langId && x.PageId == pageId).FirstOrDefaultAsync();

            return media;
        }
    }
}
