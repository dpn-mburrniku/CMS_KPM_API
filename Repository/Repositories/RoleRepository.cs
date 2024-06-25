using Entities.Models; using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Entities.Models;

namespace Repository.Repositories
{
    public class RoleRepository : GenericRepository<AspNetRole>, IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        public RoleRepository(CmsContext cmsContext
            , RoleManager<ApplicationRole> roleManager
            ) : base(cmsContext)
        {
            this.roleManager = roleManager;
        }

        public async Task<ApplicationRole> AddRole(AddRole model)
        {
            ApplicationRole newRole = new()
            {
                Name = model.Name_SQ,
                Name_SQ = model.Name_SQ,
                Name_EN = model.Name_EN,
                Name_SR = model.Name_SR,
                Description = model.Description
            };

            var result = await roleManager.CreateAsync(newRole);
            if (result.Succeeded) {
                return newRole;
            }
            return newRole;

        }

        public async Task<bool> UpdateRole(UpdateRole model)
        {
            ApplicationRole role = await roleManager.FindByIdAsync(model.Id);
            if (role != null)
            {
                role.Name = model.Name_SQ;
                role.Name_SQ = model.Name_SQ;
                role.Name_EN = model.Name_EN;
                role.Name_SR = model.Name_SR;
                role.Description = model.Description;

                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
               

        public async Task<bool> AddLayoutInRole(LayoutRole model)
        {
            try
            {
                bool exists = await _cmsContext.LayoutRoles.Where(x=>x.RoleId == model.RoleId && x.LayoutId == model.LayoutId).CountAsync() > 0;
                if (exists)
                {
                    return false;
                }
                await _cmsContext.LayoutRoles.AddAsync(model);
                await _cmsContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveLayoutFromRole(int Id)
        {
            var layoutRole = _cmsContext.LayoutRoles.FirstOrDefault(x => x.Id == Id);
            if (layoutRole != null)
            {
                 _cmsContext.Remove(layoutRole);
                _cmsContext.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> AddMenuInRole(SysMenuRole model)
        {
            try
            {
                bool exists = await _cmsContext.SysMenuRoles.Where(x => x.RoleId == model.RoleId && x.SysMenuId == model.SysMenuId).CountAsync() > 0;
                if (exists)
                {
                    return false;
                }
                await _cmsContext.SysMenuRoles.AddAsync(model);
                await _cmsContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddMenuCollectionInRole(AddMenuCollectionInRole model, string UserId)
        {
            try
            {
                foreach(var item in model.SysMenuId)
                {
                    bool exists = await _cmsContext.SysMenuRoles.Where(x => x.RoleId == model.RoleId && x.SysMenuId == item).CountAsync() > 0;
                    if (exists)
                    {
                        return false;
                    }

                    SysMenuRole menuInRole = new()
                    {
                        RoleId = model.RoleId,
                        SysMenuId = item,
                        Created = DateTime.Now,
                        CreatedBy = UserId
                    };
                    _cmsContext.SysMenuRoles.Add(menuInRole);
                    await _cmsContext.SaveChangesAsync();
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<MenusForRoleDto>> GetMenusForRole(string RoleId, int userLanguage)
        {
            var list = await _cmsContext.SysMenuRoles.Include(x => x.SysMenu).Where(x => x.RoleId == RoleId)
                .Select(t => new MenusForRoleDto
                {
                    Id = t.Id,                    
                    Name = userLanguage == 2 ? t.SysMenu.NameEn : (userLanguage == 3 ? t.SysMenu.NameSr : t.SysMenu.NameSq),   
                    Path = t.SysMenu.Path,
                    Icon = t.SysMenu.Icon,
                    Type = t.SysMenu.Type,
                    OrderNo = t.SysMenu.OrderNo,
                }).ToListAsync();
            return list;
        }

        public async Task<bool> RemoveMenuFromRole(int Id)
        {
            var menuRole = await _cmsContext.SysMenuRoles.Include(x => x.Role).Include(x => x.SysMenu).FirstOrDefaultAsync(x => x.Id == Id);
            if (menuRole != null)
            {
                _cmsContext.Remove(menuRole);
                await _cmsContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveMenuCollectionFromRole(List<int> Ids)
        {
            if (Ids.Count > 0)
            {
                foreach(int Id in Ids)
                {
                    var menuRole = await _cmsContext.SysMenuRoles.Include(x => x.Role).Include(x => x.SysMenu).FirstOrDefaultAsync(x => x.Id == Id);
                    if (menuRole != null)
                    {
                        _cmsContext.Remove(menuRole);
                        await _cmsContext.SaveChangesAsync();                        
                    }
                }
                return true;
            }
            
            return false;
        }

        public async Task<List<MenusForRoleDto>> GetMenusForRoleNotIn(string RoleId, int userLanguage, int TypeId, int ParentId)
        {            
            var MenusInThisRole = _cmsContext.SysMenuRoles.Where(x => x.RoleId == RoleId).Select(x=>x.SysMenuId).ToList();
            var list = await _cmsContext.SysMenus.Where(m=> !MenusInThisRole.Contains(m.Id)
                                && (TypeId > 0 ? m.Type == TypeId : true)
                                && (ParentId > 0 ? m.ParentId == ParentId : true  )                              
                                )
                              .Select(t => new MenusForRoleDto
                              {
                                  Id = t.Id,
                                  Name = userLanguage == 2 ? t.NameEn : (userLanguage == 3 ? t.NameSr : t.NameSq),
                                  Path = t.Path,
                                  Icon = t.Icon,
                                  Type = t.Type,
                                  OrderNo = t.OrderNo,
                              }).ToListAsync();

            return list;
        }

        public async Task<PagedList<AspNetRole>> GetRolesAsync(FilterParameters Parameters)
        {
            IQueryable<AspNetRole> roleList = _cmsContext.AspNetRoles
                            //.FilterRole(Parameters.Filter)
                            .Search(Parameters.Query, Parameters.webLangId)
                            .Sort(Parameters.Sort.key + " " + Parameters.Sort.order);
                            //.ToListAsync();

            return PagedList<AspNetRole>
            .ToPagedList(roleList, Parameters.PageIndex,
            Parameters.PageSize);
        }

        public async Task<List<AspNetRole>> GetAllRoles()
        {
            var roleList = await _cmsContext.AspNetRoles.ToListAsync();           

            return roleList;
        }
    }
}
