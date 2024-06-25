using Contracts.IServices;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
	public class MunicipalityLocationRepository : GenericRepository<MunicipalityLocation>, IMunicipalityLocationRepository
	{
		public MunicipalityLocationRepository(CmsContext cmsContext) : base(cmsContext)
		{
		}


        public async Task<IEnumerable<Layout>> GetLayoutsByRole(string RoleId, bool trackChanges, string[]? includes)
        {
            var layoutsId = _cmsContext.LayoutRoles.Where(x => x.RoleId == RoleId).AsNoTracking().Select(x => x.LayoutId).Distinct().ToList();

            var result = !trackChanges ? _cmsContext.Layouts.Where(x => layoutsId.Contains(x.Id)).IgnoreAutoIncludes().AsNoTracking() : _cmsContext.Layouts.Where(x => layoutsId.Contains(x.Id));

            return result;
        }
        public async Task<MunicipalityLocation?> GetMunicipalityLocationById(int id)
		{
			var municipalityLocation = await _cmsContext.MunicipalityLocations.Include(x => x.Media).Where(x => x.Id == id).FirstOrDefaultAsync();
			
			return municipalityLocation;

		}

		public async Task<PagedList<MunicipalityLocation>> GetMunicipalityLocationAsync(MunicipalityLocationParameters parameter)
		{
			IQueryable<MunicipalityLocation> data = _cmsContext.MunicipalityLocations.Include(x => x.Municipality).Include(x => x.MeasureUnit).Include(x => x.Media).IgnoreAutoIncludes().AsNoTracking()
													.FilterMunicipalityLocations(parameter.MunicipalityId)
													.FilterMeasureUnits(parameter.MeasureUnitId)
													.Search(parameter.Query, parameter.webLangId)
													.Sort(parameter.Sort.key + " " + parameter.Sort.order);

			return PagedList<MunicipalityLocation>
						.ToPagedList(data, parameter.PageIndex,
						parameter.PageSize);
		}
	}
}
