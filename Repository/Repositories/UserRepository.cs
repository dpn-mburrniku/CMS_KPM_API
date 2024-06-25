using Entities.Models; using CMS.API;
using CMS.API.Helpers;
using Contracts.IServices;
using Entities;
using Entities.DataTransferObjects;
//using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
//using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UserRepository : GenericRepository<AspNetUser>, IUserRespository
    {
        private UserManager<ApplicationUser> userManager;
        private IBaseRepository _baseRepository;
        public UserRepository(CmsContext cmsContext      
               , UserManager<ApplicationUser> userManager
               , IBaseRepository baseRepository

            ) : base(cmsContext)
        {
            this.userManager = userManager;
            _baseRepository = baseRepository;
        }
        public async Task<PagedList<AspNetUser>> GetUsersAsync(FilterParameters usersParameters)
        {
            var lang = _baseRepository.GetCurrentUserLanguage();
            IQueryable<AspNetUser> usersList = _cmsContext.AspNetUsers
                            //.FilterUsers(usersParameters.Filter)
                            .Search(usersParameters.Query)
                            .Sort(usersParameters.Sort.key + " " + usersParameters.Sort.order);
                            //.ToListAsync();

            return PagedList<AspNetUser>
            .ToPagedList(usersList, usersParameters.PageIndex,
            usersParameters.PageSize);
        }        

        public async Task<AspNetUser> GetUserById(string userId)
        {
            return await _cmsContext.AspNetUsers.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckUsername(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var result = await _cmsContext.AspNetUsers.Where(x => x.UserName == username).ToListAsync();
                if (result.Count > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> CheckEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var result = await _cmsContext.AspNetUsers.Where(x => x.Email == email).ToListAsync();
                if (result.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<GetProfileDto> GetProfile(string userId)
        {
            var Profili = await _cmsContext.AspNetUsers
                            .Where(x => x.Id.Equals(userId))
                            .Select(x => new GetProfileDto
                            {                                
                                Firstname = x.Firstname,
                                Lastname = x.Lastname,
                                UserName = x.UserName,
                                Email = x.Email,
                                PersonalNumber = x.PersonalNumber,
                                ProfileImage = x.ProfileImage,
                                PhoneNumber = x.PhoneNumber,
                                Language =  (LanguageEnum)x.Language,
                                Birthdate = x.Birthdate.Value.ToString("dd/MM/yyyy"),
                                Gender = (GenderEnum)x.Gender,
                                WorkPosition = x.WorkPosition
                            }).FirstOrDefaultAsync();
            return Profili;
                            
                            
                            
        }

        public async Task<ErrorStatus> PostProfile(ProfileDto model, string UserId, string Filename)
        {
            try
            {

                ApplicationUser editUser = await userManager.FindByIdAsync(UserId);
                DateTime? dtBirthdate = null;
                if (!string.IsNullOrEmpty(model.Birthdate))
                {
                    try
                    {
                        dtBirthdate = _baseRepository.StringToDate(model.Birthdate);
                    }
                    catch (Exception)
                    {                        
                    }
                }
                if (editUser != null)
                {
                    editUser.Firstname = model.Firstname;
                    editUser.Lastname = model.Lastname;
                    editUser.Email = model.Email;
                    editUser.PhoneNumber = model.PhoneNumber;
                    editUser.PersonalNumber = model.PersonalNumber;
                    editUser.Language = model.Language;
                    editUser.Birthdate = dtBirthdate.HasValue ? dtBirthdate : null;
                    editUser.Gender = model.Gender;
                    editUser.WorkPosition = model.WorkPosition;
                    if(!string.IsNullOrEmpty(Filename))
                    {
                        editUser.ProfileImage = Filename;
                    }                    

                    await userManager.UpdateAsync(editUser);
                    return ErrorStatus.Success;
                }
                return ErrorStatus.Error;
            }
            catch (Exception)
            {
                return ErrorStatus.Error;
            }

        }

        public async void AddUserAudit(byte ActionType, string UserId, string ActionSq, string ActionEn, string ActionSr)
        {
            try
            {
                var data = new UserAudit()
                {
                    UserId = UserId ?? "N/A",
                    ActionType = ActionType,
                    DescriptionSq = ActionSq,
                    DescriptionEn = ActionEn,
                    DescriptionSr = ActionSr,
                    Date = DateTime.Now
                };

                _cmsContext.Add(data);
                await _cmsContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
        }
    }

}
