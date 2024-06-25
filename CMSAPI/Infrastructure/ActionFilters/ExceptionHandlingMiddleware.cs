using CMS.API.Helpers;
using Entities.DataTransferObjects.WebDTOs;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.ComponentModel;
using Contracts.IServices;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using static System.Net.Mime.MediaTypeNames;
using System;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace CMS.API.Infrastructure.ActionFilters
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;        

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;            
        }

        public async Task InvokeAsync(HttpContext httpContext, CmsContext _context, IBaseRepository _baseRepository)
        {
            try
            {
                httpContext.Request.EnableBuffering();
                await _next(httpContext);
                if (httpContext.Request.Method.ToUpper() != "GET" && httpContext.Request.RouteValues.Count > 0)
                {
                    if (!httpContext.Request.RouteValues["action"].ToString().ToUpper().Contains("GET") && !httpContext.Request.RouteValues["action"].ToString().ToUpper().Contains("LOGIN")
                        && !httpContext.Request.RouteValues["action"].ToString().ToUpper().Contains("LOGOUT"))
                    {
                        await OnActionExecutionAsync(httpContext, _context, _baseRepository);
                    }

                }                

            }
            catch (Exception ex)
            {
                await _next(httpContext);
                await HandleExceptionAsync(httpContext, ex, _context, _baseRepository);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception, CmsContext _context, IBaseRepository _baseRepository)
        {
            try
            {
                _context.ChangeTracker.Clear();
                var log = new Log();                

                log.Action = context.Request.RouteValues["action"].ToString();
                log.ActionDescription = context.Request.RouteValues["action"].ToString();
                log.IsError = true;
                log.Controller = context.Request.RouteValues["controller"].ToString();
                log.InsertedDate = DateTime.Now;
                log.UserId = _baseRepository.GetLoggedUserId() ?? "";
                log.UserName = _baseRepository.GetLoggedUsername() ?? "";
                log.Ip = context.Connection.RemoteIpAddress.ToString();
                log.Hostname = context.Connection.RemoteIpAddress.ToString();                
                //log.DescriptionTitle = description.Title;
                log.HttpMethod = context.Request.Method;
                log.Url = context.Request.GetDisplayUrl();
                log.Exception = JsonConvert.SerializeObject(exception);                
                if (context.Request.HasFormContentType)
                {
                    IFormCollection form = await context.Request.ReadFormAsync();
                    log.FormContent = JsonConvert.SerializeObject(form);
                }

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                
                //context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            catch (Exception ex)
            {               
            }
        }

        public async Task OnActionExecutionAsync(HttpContext context, CmsContext _context, IBaseRepository _baseRepository)
        {

            Log log = new Log();

            log.Action = context.Request.RouteValues["action"].ToString();
            log.ActionDescription = context.Request.RouteValues["action"].ToString();
            log.IsError = false;
            log.Controller = context.Request.RouteValues["controller"].ToString();
            log.InsertedDate = DateTime.Now;
            log.UserId = _baseRepository.GetLoggedUserId() ?? "";
            log.UserName = _baseRepository.GetLoggedUsername() ?? "";
            log.Ip = context.Connection.RemoteIpAddress.ToString();
            log.Hostname = context.Connection.RemoteIpAddress.ToString();
            //log.Hostname = Environment.MachineName ?? "N/A";
            // log.Description = description.Description;
            //log.DescriptionTitle = description.Title;
            log.Url = context.Request.GetDisplayUrl();
            log.HttpMethod = context.Request.Method;

            if (context.Request.HasFormContentType)
            {
                IFormCollection form = await context.Request.ReadFormAsync();
                log.FormContent = JsonConvert.SerializeObject(form);
            }
            else if (context.Request.QueryString.HasValue)
            {
                log.FormContent = JsonConvert.SerializeObject(context.Request.QueryString);
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        context.Request.Body.Seek(0, SeekOrigin.Begin);

                        context.Request.Body.CopyTo(stream);

                        context.Request.Body.Seek(0, SeekOrigin.Begin);

                        var contentType = context.Request.ContentType;

                        if (contentType != null && contentType.ToLower().StartsWith("application/x-www-form-urlencoded"))
                        {
                            var formCollection = new FormCollection(await new FormReader(stream).ReadFormAsync());

                            var formContent = string.Join("&", formCollection
                                .Where(kvp => kvp.Key.ToLower() != "file")
                                .Select(kvp => $"{kvp.Key}={kvp.Value}"));

                            log.FormContent = formContent;
                        }
                        else
                        {
                            log.FormContent = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        log.FormContent = "";
                    }
                }
            }

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
               
    }
}
