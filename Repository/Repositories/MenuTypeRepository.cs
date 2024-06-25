using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class MenuTypeRepository : GenericRepository<MenuType>, IMenuTypeRepository
    {
        public MenuTypeRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {
           
        }

        public async Task<MenuType?> GetMenuTypeById(int id, int webLangId)
        {
            var data = await _cmsContext.MenuTypes.FindAsync(id, webLangId);

            return data;
        }

    }
}
