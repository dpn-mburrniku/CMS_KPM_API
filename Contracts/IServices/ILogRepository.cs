using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
    public interface ILogRepository : IGenericRepository<Log>
    {
        Task<PagedList<Log>> GetLogsAsync(LogFilterParameters parameter);
        PagedList<Log> GetUserAuditAsync(UserAuditFilterParameters parameter);
    }
}
