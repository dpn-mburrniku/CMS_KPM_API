using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
	public interface IMunicipalityLocationRepository : IGenericRepository<MunicipalityLocation>
	{
			Task<MunicipalityLocation?> GetMunicipalityLocationById(int id);

			Task<PagedList<MunicipalityLocation>> GetMunicipalityLocationAsync(MunicipalityLocationParameters parameter);

	}
}
