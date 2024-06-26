﻿using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Abp.Linq.Expressions;

namespace Repository.Repositories
{
    public class LayoutRepository : GenericRepository<Layout>, ILayoutRepository
    {
        public LayoutRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }       

        public async Task<IEnumerable<Layout>> GetByIdsAsync(IEnumerable<int> ids, bool trackChanges, string[]? includes) =>
                await FindByCondition(x => ids.Contains(x.Id), trackChanges, includes)
                .ToListAsync();

        public async Task<IEnumerable<Layout>> GetLayoutsForRoleNotIn(string RoleId, bool trackChanges, string[]? includes)
        {
            var layoutsId = _cmsContext.LayoutRoles.Where(x => x.RoleId == RoleId).AsNoTracking().Select(x => x.LayoutId).Distinct().ToList();

            var layoutsQuery = PredicateBuilder.False<Layout>();

            foreach (var id in layoutsId)
            {
                layoutsQuery = layoutsQuery.Or(t => t.Id == id);
            }

            var result = !trackChanges ? _cmsContext.Layouts.Where(layoutsQuery).IgnoreAutoIncludes().AsNoTracking() : _cmsContext.Layouts.Where(layoutsQuery);

            if (includes != null)
            {
                foreach (var property in includes)
                {
                    result = result.Include(property);
                }
            }

            return result;
        }
        public async Task<IEnumerable<Layout>> GetLayoutsByRole(string RoleId, bool trackChanges, string[]? includes)
        {
            var layoutsId = _cmsContext.LayoutRoles.Where(x => x.RoleId == RoleId).AsNoTracking().Select(x => x.LayoutId).Distinct().ToList();

            var layoutsQuery = PredicateBuilder.False<Layout>();

            foreach (var id in layoutsId)
            {
                layoutsQuery = layoutsQuery.Or(t => t.Id == id);
            }

            var result = !trackChanges ? _cmsContext.Layouts.Where(layoutsQuery).IgnoreAutoIncludes().AsNoTracking() : _cmsContext.Layouts.Where(layoutsQuery);

            return result;
        }

        public async Task<IEnumerable<LayoutRole>> GetRoleLayouts(string RoleId, bool trackChanges, string[]? includes)
        {
            var result = trackChanges ? _cmsContext.LayoutRoles.Where(x => x.RoleId == RoleId) :
            _cmsContext.LayoutRoles.Where(x => x.RoleId == RoleId).IgnoreAutoIncludes().AsNoTracking();

            if (includes != null)
            {
                foreach (var property in includes)
                {
                    result = result.Include(property);
                }
            }

            return result;
        }

        public async Task<PagedList<Layout>> GetLayoutsAsync(FilterParameters parameter)
        {
            IQueryable<Layout> data =  _cmsContext.Layouts
                            .Search(parameter.Query, parameter.webLangId)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);

            return PagedList<Layout>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

    }
}
