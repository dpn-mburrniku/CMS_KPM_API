using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CMS.API.Infrastructure.ActionFilters
{
    public class ValidateLayoutExistsAttribute : IAsyncActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;

        public ValidateLayoutExistsAttribute(IUnitOfWork unitOfWork,
        ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (int)context.ActionArguments["id"];

            var company = await _unitOfWork.Layouts.GetById(id);

            if (company == null)
            {
                _logger.LogInfo($"Layout with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("layout", company);
                await next();
            }
        }
    }

}
