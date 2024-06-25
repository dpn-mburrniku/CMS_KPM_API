using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface IUserActivityRepository : IGenericRepository<UserAudit>
    {
        Task<PagedList<UserAudit>> GetUserAuditAsync(UserActivitiesFilterParameters parameter);
    }
}
