using Entities.Models; using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Entities.RequestFeatures;
using Repository.Extensions;

namespace Repository.Repositories
{
    public class SysMenuRepository : GenericRepository<SysMenu>, ISysMenuRepository
    {
        public SysMenuRepository(CmsContext cmsContext
            ) : base(cmsContext)
        {

        }
               
        public async Task<List<SysMenuDto>> GetSysMenu(int userLanguage)
        {

            var list = await (from m in _cmsContext.SysMenus
                              where m.Active == true
                              && m.ParentId == null
                              select new SysMenuDto
                              {
                                  Key = m.Id,
                                  ParentId = m.ParentId,
                                  Title = userLanguage == 2 ? m.NameEn : (userLanguage == 3 ? m.NameSr : m.NameSq),
                                  Path = m.Path,
                                  Icon = m.Icon,
                                  Type = m.Type,
                                  OrderNo = m.OrderNo,
                                  Active = m.Active,
                                  Authority = (from r in _cmsContext.AspNetRoles
                                               join mr in _cmsContext.SysMenuRoles on r.Id equals mr.RoleId
                                               where mr.SysMenuId == m.Id
                                               select r.Name
                                               ).ToList(),
                                  HasSubMenu = _cmsContext.SysMenus.Where(x=> x.Active == true && x.ParentId == m.Id).Count() > 0 ? true : false,
                                  SubMenu = (from sb in _cmsContext.SysMenus
                                             where sb.Active == true && sb.ParentId == m.Id
                                             select new SysMenuDto
                                             {
                                                 Key = sb.Id,
                                                 ParentId = sb.ParentId,
                                                 Title = userLanguage == 2 ? sb.NameEn : (userLanguage == 3 ? sb.NameSr : sb.NameSq),
                                                 Path = sb.Path,
                                                 Icon = sb.Icon,
                                                 Type = sb.Type,
                                                 OrderNo = sb.OrderNo,
                                                 Active = sb.Active,
                                                 Authority = (from r in _cmsContext.AspNetRoles
                                                              join mr in _cmsContext.SysMenuRoles on r.Id equals mr.RoleId
                                                              where mr.SysMenuId == sb.Id
                                                              select r.Name
                                                              ).ToList(),
                                                 HasSubMenu = _cmsContext.SysMenus.Where(x => x.Active == true && x.ParentId == sb.Id).Count() > 0 ? true : false,
                                                 SubMenu = (from sb1 in _cmsContext.SysMenus
                                                            where sb1.Active == true && sb1.ParentId == sb.Id
                                                            select new SysMenuDto
                                                            {
                                                                Key = sb1.Id,
                                                                ParentId = sb1.ParentId,
                                                                Title = userLanguage == 2 ? sb1.NameEn : (userLanguage == 3 ? sb1.NameSr : sb1.NameSq),
                                                                Path = sb1.Path,
                                                                Icon = sb1.Icon,
                                                                Type = sb1.Type,
                                                                OrderNo = sb1.OrderNo,
                                                                Active = sb1.Active,
                                                                Authority = (from r1 in _cmsContext.AspNetRoles
                                                                             join mr1 in _cmsContext.SysMenuRoles on r1.Id equals mr1.RoleId
                                                                             where mr1.SysMenuId == sb1.Id
                                                                             select r1.Name
                                                                             ).ToList(),
                                                            }).OrderBy(x => x.OrderNo).ToList()
                                             }).OrderBy(x => x.OrderNo).ToList()
                              }).OrderBy(x => x.OrderNo).ToListAsync();
            return list;
        }

        public async Task<PagedList<SysMenu>> GetAllMenuAsync(FilterParameters parameter)
        {
            IQueryable<SysMenu> data =  _cmsContext.SysMenus.Include(x => x.Parent).IgnoreAutoIncludes().AsNoTracking()
                            .Search(parameter.Query, parameter.webLangId)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);

            return PagedList<SysMenu>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }
    }
}
