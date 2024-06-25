using Entities.Models; using CMS.API;
using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IMenuTypeRepository : IGenericRepository<MenuType>
    {
        Task<MenuType?> GetMenuTypeById(int id, int webLangId);
    }


}
