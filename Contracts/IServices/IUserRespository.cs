//using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Models; using CMS.API;
using CMS.API.Helpers;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;

namespace Contracts.IServices
{
    public interface IUserRespository : IGenericRepository<AspNetUser>
    {
        Task<PagedList<AspNetUser>> GetUsersAsync(FilterParameters usersParameters);
        Task<bool> CheckUsername(string username);
        Task<bool> CheckEmail(string email);
        Task<AspNetUser> GetUserById(string userId);
        Task<GetProfileDto> GetProfile(string userId);
        Task<ErrorStatus> PostProfile(ProfileDto model, string UserId, string Filename);
        //Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges);
        //Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        //Task<PagedList<UserDto>> GetUserAsync(Guid userId, EmployeeParameters employeeParameters);
        //void CreateEmployeeForCompany(Guid companyId, Employee employee);
        //void DeleteEmployee(Employee employee);
        void AddUserAudit(byte ActionType, string UserId, string ActionSq, string ActionEn, string ActionSr);
    }
}
