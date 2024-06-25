using Entities.Models; using Entities.Models; using CMS.API;
using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Entities.RequestFeatures;

namespace Contracts.IServices
{
    public interface ISysMenuRepository : IGenericRepository<SysMenu>
    {
        Task<List<SysMenuDto>> GetSysMenu(int userLanguage);

        Task<PagedList<SysMenu>> GetAllMenuAsync(FilterParameters parameter);

    }
}
