using Entities.Models; 
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<bool> AddCollectionPagesInMenu(AddCollectionMenu model, string userId);
        Task<Menu?> GetMenuById(int id, int webLangId);



        // web interfaces
        Task<List<MenuModel>> GetMenus(int GjuhaID, int MenuTypeID, int? ParentMenuID);
        Task<List<MenuModel>> GetSideBarMenus(int GjuhaID, int MenuTypeID, int? ParentMenuID, int ParentPageID);
        Task<List<MenuModel>> GetMenusWithoutChilds(int GjuhaID, int MenuTypeID, int? ParentMenuID);
        Task<List<MenuModel>> GetMenuPath(int PageID, int GjuhaID, int MenuTypeID, int niveli);
        Task<List<MenuModel>> GetMenuPathLayoutHome(int LayoutID, int GjuhaID, int MenuTypeID, int niveli);
        Task<List<MenuAtoZModel>> GetMenusAtoZ(int GjuhaID);
        Task<List<MenuAtoZModel>> GetMenusAtoZWithLetter(int GjuhaID, string Letter);
        Task<List<MenuModel>> GetMenuByPageID(int PageID, int GjuhaID);
        Task<List<MenuModel>> GetMenuByPageID(int PageID, int GjuhaID, int MenuTypeID);
    }
}
