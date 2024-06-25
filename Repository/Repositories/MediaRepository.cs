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
using Microsoft.AspNetCore.Identity;

namespace Repository.Repositories
{
    public class MediaRepository : GenericRepository<Medium>, IMediaRepository
    {
        public MediaRepository(CmsContext cmsContext
           ) : base(cmsContext)
        {

        }

        public async Task<PagedList<Medium>> GetMediaAsync(MediaFilterParameters parameter)
        {
            DateTime? dtFrom = new();
            DateTime? dtTo = new();
            if (!string.IsNullOrEmpty(parameter.DateFrom))
            {
                try
                {
                    parameter.DateFrom = parameter.DateFrom.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateFrom.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateFrom.Split('/')[1]);
                    int viti = int.Parse(parameter.DateFrom.Split('/')[2]);
                    dtFrom = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtFrom = null;
            }
            if (!string.IsNullOrEmpty(parameter.DateTo))
            {
                try
                {
                    parameter.DateTo = parameter.DateTo.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateTo.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateTo.Split('/')[1]);
                    int viti = int.Parse(parameter.DateTo.Split('/')[2]);
                    dtTo = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtTo = null;
            }
            IQueryable<Medium> data = _cmsContext.Media.Include(x => x.MediaExCategory).Include(x => x.FileExNavigation).AsNoTracking()
                            .FilterMediaByMediaExCategory(parameter.MediaExCategoryId)
                            .FilterMediaByCreateDate(dtFrom, dtTo)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);
                            //.ToListAsync();

            return PagedList<Medium>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public async Task<List<string>> GetMediaEx()
        {
            var mediaList = await _cmsContext.MediaExes.Select(x => x.MediaEx1).ToListAsync();
            return mediaList;
        }

        public async Task<MediaEx> GetMediaExCategory(string mediaEx)
        {
            var list = await _cmsContext.MediaExes.Include(x => x.MediaExCategory).Where(x => x.MediaEx1 == mediaEx).FirstOrDefaultAsync();
            return list;
        }
    }
}
