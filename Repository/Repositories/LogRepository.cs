using Contracts.IServices;
using Entities.Models;
using Entities.RequestFeatures;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class LogRepository : GenericRepository<Log>, ILogRepository
    {
        public LogRepository(CmsContext cmsContext) : base(cmsContext)
        {
        }

        public async Task<PagedList<Log>> GetLogsAsync(LogFilterParameters parameter)
        {
            DateTime? dtFrom = new();
            DateTime? dtTo = new();
            if (!string.IsNullOrEmpty(parameter.DateFrom))
            {
                try
                {
                    parameter.DateFrom = parameter.DateFrom.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateFrom.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateFrom.Split('/')[1]);
                    int viti = int.Parse(parameter.DateFrom.Split('/')[2]);
                    dtFrom = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtFrom = null;
            }
            if (!string.IsNullOrEmpty(parameter.DateTo))
            {
                try
                {
                    parameter.DateTo = parameter.DateTo.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateTo.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateTo.Split('/')[1]);
                    int viti = int.Parse(parameter.DateTo.Split('/')[2]);
                    dtTo = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtTo = null;
            }

            IQueryable<Log> data = _cmsContext.Logs
                            .FilterLogsByDate(dtFrom, dtTo)
                            .FilterLogsByUsername(parameter.Username)
                            .FilterLogsByMethod(parameter.Method)
                            .FilterLogsByController(parameter.Controller)
                            .FilterIsError(parameter.IsError)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);

            return PagedList<Log>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public PagedList<Log> GetUserAuditAsync(UserAuditFilterParameters parameter)
        {
            DateTime? dtFrom = new();
            DateTime? dtTo = new();
            if (!string.IsNullOrEmpty(parameter.DateFrom))
            {
                try
                {
                    parameter.DateFrom = parameter.DateFrom.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateFrom.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateFrom.Split('/')[1]);
                    int viti = int.Parse(parameter.DateFrom.Split('/')[2]);
                    dtFrom = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtFrom = null;
            }
            if (!string.IsNullOrEmpty(parameter.DateTo))
            {
                try
                {
                    parameter.DateTo = parameter.DateTo.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(parameter.DateTo.Split('/')[0]);
                    int muaji = int.Parse(parameter.DateTo.Split('/')[1]);
                    int viti = int.Parse(parameter.DateTo.Split('/')[2]);
                    dtTo = new DateTime(viti, muaji, dita);
                }
                catch (Exception)
                {

                }
            }
            else
            {
                dtTo = null;
            }

            var user = _cmsContext.AspNetUsers.Where(x => x.Id == parameter.UserId).FirstOrDefault();

            IQueryable<Log> data = _cmsContext.Logs
                            .FilterLogsByDate(dtFrom, dtTo)
                            .FilterLogsByUsername(user.UserName)
                            .FilterIsError(false)
                            .Search(parameter.Query)
                            .Sort(parameter.Sort.key + " " + parameter.Sort.order);

            return PagedList<Log>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }
    }
}
