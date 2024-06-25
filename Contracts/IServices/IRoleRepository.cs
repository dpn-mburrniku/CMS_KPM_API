using Entities.Models;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;

namespace Contracts.IServices
{
    public interface IRoleRepository : IGenericRepository<AspNetRole>
    {
        Task<ApplicationRole> AddRole(AddRole model);
        Task<bool> UpdateRole(UpdateRole model);        
        Task<bool> AddLayoutInRole(LayoutRole model);
        bool RemoveLayoutFromRole(int Id);        
        Task<List<MenusForRoleDto>> GetMenusForRole(string RoleId, int userLanguage);
        Task<bool> AddMenuInRole(SysMenuRole model); 
        Task<bool> AddMenuCollectionInRole(AddMenuCollectionInRole model, string UserId);
        Task<bool> RemoveMenuFromRole(int Id);
        Task<bool> RemoveMenuCollectionFromRole(List<int> Ids);
        Task<List<MenusForRoleDto>> GetMenusForRoleNotIn(string RoleId, int userLanguage, int TypeId, int ParentId);
        Task<PagedList<AspNetRole>> GetRolesAsync(FilterParameters Parameters);
        Task<List<AspNetRole>> GetAllRoles();
    }
}
