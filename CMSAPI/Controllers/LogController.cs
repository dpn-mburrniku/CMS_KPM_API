using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;
using CMS.API.Helpers;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public LogController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Auditimet
        [Authorize]
        [HttpPost]
        [Route("GetAuditLogsAsync")]
        public async Task<IActionResult> GetAuditLogsAsync([FromBody] LogFilterParameters parameter)
        {
            var logs = await _unitOfWork.Logs.GetLogsAsync(parameter);
            var logsDto = _mapper.Map<IEnumerable<LogsDto>?>(logs);

            return Ok(new
            {
                data = logsDto,
                total = logs.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpGet]
        [Route("GetControllerList")]
        public async Task<IActionResult> GetControllerList()
        {
            var data = await _unitOfWork.Logs.FindAll(false, null).Select(x => x.Controller).Distinct().ToListAsync();

            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        [Route("GetHttpMethodList")]
        public async Task<IActionResult> GetHttpMethodList()
        {
            var data = await _unitOfWork.Logs.FindAll(false, null).Select(x => x.HttpMethod).Distinct().ToListAsync();

            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUsernameList")]
        public async Task<IActionResult> GetUsernameList()
        {
            var data = await _unitOfWork.Logs.FindAll(false, null).Where(x => x.UserName != null).Select(x => x.UserName).Distinct().ToListAsync();

            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUsernameListForActivities")]
        public async Task<IActionResult> GetUsernameListForActivities()
        {
            var data = await _unitOfWork.Users.FindAll(false, null).Select(x => x.UserName).Distinct().ToListAsync();

            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        [Route("GetActionTypeList")]
        public async Task<IActionResult> GetActionTypeList()
        {
            var data = Enum.GetNames(typeof(ActionType))
            .Select(i => new
            {
                description = i,
                value = (Enum.Parse(typeof(ActionType), i.ToString()))
            });

            return Ok(data);
        }

        #endregion


        #region user Audit and activities

        [Authorize]
        [HttpPost]
        [Route("GetUserAuditAsync")]
        public async Task<IActionResult> GetUserAuditAsync([FromBody] UserAuditFilterParameters parameter)
        {
            var data = _unitOfWork.Logs.GetUserAuditAsync(parameter);
            var dataDto = _mapper.Map<IEnumerable<LogsDto>?>(data);

            return Ok(new
            {
                data = dataDto,
                total = data.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpPost]
        [Route("GetUserActivitiesAsync")]
        public async Task<IActionResult> GetUserActivitiesAsync([FromBody] UserActivitiesFilterParameters parameter)
        {
            if(!string.IsNullOrEmpty(parameter.Username))
            {
                var username = _unitOfWork.Users.FindByCondition(x => x.UserName.ToLower() == parameter.Username.ToLower(), false, null).FirstOrDefault();
                parameter.Username = username.Id;
            }
             
            var data = await _unitOfWork.UserActivity.GetUserAuditAsync(parameter);
            var dataDto = _mapper.Map<IEnumerable<UserActivitiesDto>?>(data);

            foreach(var item in dataDto)
            {
                if(!string.IsNullOrEmpty(item.UserId) && item.UserId.ToLower() != "n/a" )
                {
                    item.UserName = _unitOfWork.Users.FindByCondition(x => x.Id == item.UserId, false, null).FirstOrDefault().UserName;
                }       
                else
                {
                    item.UserName = "N/A";
                }
                
            }
            return Ok(new
            {
                data = dataDto,
                total = data.MetaData.TotalCount
            });
        }


        #endregion        
                

    }
}