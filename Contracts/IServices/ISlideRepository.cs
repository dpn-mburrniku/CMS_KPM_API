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
    public interface ISlideRepository : IGenericRepository<Slide>
    {
        Task<PagedList<Slide>> GetSlideAsync(FilterParameters parameter, bool isDeleted);
        Task<Slide> GetSlideById(int id, int webLangId);

        //web
        Task<List<SlidesModel>> GetSlides(int GjuhaId, int PageID);
    }


}
