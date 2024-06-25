using Entities.Models; using CMS.API;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Contracts.IServices
{
    public interface ILayoutRepository : IGenericRepository<Layout>
    {
        Task<IEnumerable<Layout>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges, string[]? includes);
        Task<IEnumerable<LayoutRole>> GetRoleLayouts(string RoleId, bool trackChanges, string[]? includes);
        Task<IEnumerable<Layout>> GetLayoutsForRoleNotIn(string RoleId, bool trackChanges, string[]? includes);
        Task<IEnumerable<Layout>> GetLayoutsByRole(string RoleId, bool trackChanges, string[]? includes);
        Task<PagedList<Layout>> GetLayoutsAsync(FilterParameters parameter);
    }
}
