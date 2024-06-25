using Entities.Models; using CMS.API;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts.IServices
{
    public interface IFaqHeaderRepository: IGenericRepository<Faqheader>
    {
        Task<Faqheader?> GetFaqHeaderById(int id, int webLangId);
        Task<PagedList<Faqheader>> GetFaqHeaderAsync(FaqHeaderParameters parameter);
    }
}
