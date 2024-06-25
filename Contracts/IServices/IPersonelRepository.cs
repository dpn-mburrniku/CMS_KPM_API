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
    public interface IPersonelRepository : IGenericRepository<Personel>
    {
        Task<Personel?> GetPersonelById(int id, int webLangId);

        Task<PagedList<Personel>> GetPersonelAsync(PersonelFilterParameters parameter);

        //web
        Task<List<PersonelModel>> GetPersonel(int GjuhaId, int PageId, int skip, int take);
        Task<int> GetPersonelCount(int GjuhaId, int PageId);
        Task<List<PersonelModel>> GetPersonelDetails(int GjuhaId, int PersoneliID);

    }
}
