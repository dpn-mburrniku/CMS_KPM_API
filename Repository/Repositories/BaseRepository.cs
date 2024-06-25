using Entities.Models;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using CMS.API.Helpers;
using System.Security.Cryptography;
using System.Text;
using Entities.RequestFeatures;
using Repository.Extensions;

namespace Repository.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private IHttpContextAccessor _httpContextAccessor { get; }
        private UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private CmsContext _cmsContext;
        public BaseRepository(CmsContext cmsContext
            , IHttpContextAccessor httpContextAccessor
             , UserManager<ApplicationUser> userManager
             , RoleManager<ApplicationRole> roleManager
            )
        {
            _httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _cmsContext = cmsContext;
        }


        public int GetGjuhaID(string Gjuha)
        {
            int GjuhaID = 1;
            if (Gjuha == "en")
                GjuhaID = 2;
            else if (Gjuha == "sr")
                GjuhaID = 3;

            return GjuhaID;
        }
        public int GetTotalPages(int totalRowsCount, int take)
        {
            int totalPagesInt = totalRowsCount / take;
            decimal mbetja = (decimal)totalRowsCount / (decimal)take;
            int totalPages = mbetja > totalPagesInt ? totalPagesInt + 1 : totalPagesInt;

            return totalPages;
        }
        public DateTime GetDateTime()
        {
            DateTime data = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? DateTime.UtcNow.AddHours(2) : DateTime.UtcNow.AddHours(1);

            return data;
        }

        public string GetLoggedUserId()
        {
            try
            {
                if (_httpContextAccessor?.HttpContext?.User != null)
                    return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetLoggedUsername()
        {
            try
            {
                if (_httpContextAccessor?.HttpContext?.User != null)
                    return _httpContextAccessor.HttpContext.User.Identity.Name;

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetCurrentUserLanguage()
        {
            try
            {
                if (_httpContextAccessor?.HttpContext?.User != null)
                    return int.Parse(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Locality).Value);

                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime StringToDate(string rdata)
        {
            DateTime data = new DateTime();
            try
            {
                if (!string.IsNullOrEmpty(rdata))
                {
                    rdata = rdata.Replace(" ", "/").Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(rdata.Split('/')[0]);
                    int muaji = int.Parse(rdata.Split('/')[1]);
                    int viti = int.Parse(rdata.Split('/')[2]);
                    data = new DateTime(viti, muaji, dita);
                }
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        public DateTime StringToDateTime(string rdata)
        {
            DateTime data = new DateTime();
            try
            {
                if (!string.IsNullOrEmpty(rdata))
                {

                    string datastr = rdata.Split(' ')[0];
                    string kohastr = rdata.Split(' ')[1];
                    rdata = datastr.Replace(".", "/").Replace("-", "/");
                    int dita = int.Parse(rdata.Split('/')[0]);
                    int muaji = int.Parse(rdata.Split('/')[1]);
                    int viti = int.Parse(rdata.Split('/')[2]);

                    int ora = int.Parse(kohastr.Split(':')[0]);
                    int minuatat = int.Parse(kohastr.Split(':')[1]);
                    int sekondat = int.Parse(kohastr.Split(':')[2]);
                    data = new DateTime(viti, muaji, dita, ora, minuatat, sekondat);
                }
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        public async Task<List<string>> GetUserRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var userRoles = await userManager.GetRolesAsync(user);
            var roles = new List<string>();
            foreach (var userRole in userRoles)
            {
                roles.Add(userRole);
            }
            return roles;
        }

        public async Task<List<string>> GetUserRolesId(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var userRoles = await userManager.GetRolesAsync(user);
            var roles = new List<string>();
            foreach (var userRole in userRoles)
            {
                var Role = await roleManager.FindByNameAsync(userRole);
                roles.Add(Role.Id);
            }
            return roles;
        }

        public async Task<ThemeConfigDto> GetThemeConfig(int userLanguage)
        {
            var userId = GetLoggedUserId();
            var existsOnDb = await _cmsContext.ThemeConfigs.Where(x => x.UserId == userId).ToListAsync();
            if (existsOnDb.Any())
            {
                var themeConfig = await _cmsContext.ThemeConfigs.Where(x => x.UserId == userId)
                .Select(t => new ThemeConfigDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    ThemeColor = t.ThemeColor,
                    Mode = t.Mode,
                    PrimaryColorLevel = t.PrimaryColorLevel,
                    NavMode = t.NavMode,
                    LayoutType = t.LayoutType,
                    Locale = userLanguage
                }).FirstOrDefaultAsync();
                return themeConfig;
            }
            else
            {
                var themeConfig = new ThemeConfigDto
                {
                    ThemeColor = "blue",
                    Mode = "light",
                    PrimaryColorLevel = 600,
                    NavMode = "light",
                    LayoutType = "modern",
                    Locale = userLanguage
                };
                return themeConfig;
            }

        }

        public async Task<bool> UpdateThemeConfig(ThemeConfigDto model)
        {
            try
            {
                var userId = GetLoggedUserId();
                var existsOnDb = await _cmsContext.ThemeConfigs.Where(x => x.UserId == userId).ToListAsync();
                if (existsOnDb.Any())
                {
                    var data = await _cmsContext.ThemeConfigs.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.ThemeColor = model.ThemeColor;
                        data.Mode = model.Mode;
                        data.PrimaryColorLevel = model.PrimaryColorLevel;
                        data.NavMode = model.NavMode;
                        data.LayoutType = model.LayoutType;
                        _cmsContext.ThemeConfigs.Update(data);
                        await _cmsContext.SaveChangesAsync();
                    }
                    return true;
                }
                else
                {
                    var data = new ThemeConfig();
                    data.UserId = userId;
                    data.ThemeColor = model.ThemeColor;
                    data.Mode = model.Mode;
                    data.PrimaryColorLevel = model.PrimaryColorLevel;
                    data.NavMode = model.NavMode;
                    data.LayoutType = model.LayoutType;
                    _cmsContext.ThemeConfigs.Add(data);
                    await _cmsContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public int? GetLayoutID(string Layout)
        {
            int? layoutID = 1;
            var layout = _cmsContext.Layouts.Where(x => x.NameSq.Contains(Layout) || x.NameEn.Contains(Layout) || x.NameSr.Contains(Layout)).FirstOrDefault();

            if (layout != null)
            {
                layoutID = layout.Id;
            }

            return layoutID;
        }

        public async Task<List<ComponentLocation>> GetComponentLocation()
        {
            var list = await _cmsContext.ComponentLocations.Where(x => x.Active == true).ToListAsync();
            return list;
        }

        public async Task<List<LinkType>> GetLinkTypes()
        {
            var userLanguageId = GetCurrentUserLanguage();
            var list = await _cmsContext.LinkTypes.Where(x => x.LanguageId == userLanguageId).ToListAsync();
            return list;
        }
        public async Task<List<PostCategory>> GetPostCategories()
        {
            var userLanguageId = GetCurrentUserLanguage();
            var list = await _cmsContext.PostCategories.Where(x => x.LanguageId == userLanguageId).ToListAsync();
            return list;
        }
        public async Task<List<Template>> GetTemplates()
        {
            var template = await _cmsContext.Templates.ToListAsync();
            return template;
        }

        public async Task<List<Language>> GetLangList()
        {
            var list = await _cmsContext.Languages.Where(x => x.Active == true).AsNoTracking().ToListAsync();
            return list;
        }

        public async Task<List<MediaExCategory>> GetMediaExCategory()
        {
            var list = await _cmsContext.MediaExCategories.AsNoTracking().ToListAsync();
            return list;
        }

        public async Task<List<Setting>> GetSysSettings()
        {
            var list = await _cmsContext.Settings.ToListAsync();
            return list;
        }

        public async Task<Setting?> GetSettingById(int id)
        {
            var setting = await _cmsContext.Settings.FindAsync(id);

            return setting;
        }

        public async Task<PagedList<Setting>> GetExtraSettingsAsync(ExtraSettingsFilterParameters parameter)
        {
            IQueryable<Setting> data = _cmsContext.Settings.Where(x => x.Id > 21).IgnoreAutoIncludes().AsNoTracking();
            
            //.ToListAsync();

            return PagedList<Setting>
            .ToPagedList(data, parameter.PageIndex,
            parameter.PageSize);
        }

        public async Task<SettingsDto?> GetSettingsForEdit()
        {
            var settings = _cmsContext.Settings.ToList();
            if (settings != null)
            {
                var list = new SettingsDto
                {
                    SiteName = settings.Where(X => X.Label == "SiteName" && X.Value != null).Count() > 0 ? settings.Where(X => X.Label == "SiteName").FirstOrDefault().Value : "",
                    SiteDescription = settings.Where(X => X.Label == "SiteDescription" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "SiteDescription").FirstOrDefault().Value : "",
                    SiteUrl = settings.Where(X => X.Label == "SiteUrl" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "SiteUrl").FirstOrDefault().Value : "",
                    IISSiteName = settings.Where(X => X.Label == "IISSiteName" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "IISSiteName").FirstOrDefault().Value : "",
                    LocalPath = settings.Where(X => X.Label == "LocalPath" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "LocalPath").FirstOrDefault().Value : "",
                    PhotoLargeWidth = settings.Where(X => X.Label == "PhotoLargeWidth" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoLargeWidth").FirstOrDefault().Value : "",
                    PhotoLargeHeight = settings.Where(X => X.Label == "PhotoLargeHeight" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoLargeHeight").FirstOrDefault().Value : "",
                    PhotoMediumWidth = settings.Where(X => X.Label == "PhotoMediumWidth" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoMediumWidth").FirstOrDefault().Value : "",
                    PhotoMediumHeight = settings.Where(X => X.Label == "PhotoMediumHeight" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoMediumHeight").FirstOrDefault().Value : "",
                    PhotoSmallWidth = settings.Where(X => X.Label == "PhotoSmallWidth" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoSmallWidth").FirstOrDefault().Value : "",
                    PhotoSmallHeight = settings.Where(X => X.Label == "PhotoSmallHeight" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhotoSmallHeight").FirstOrDefault().Value : "",
                    Email = settings.Where(X => X.Label == "Email" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "Email").FirstOrDefault().Value : "",
                    PhoneNumber = settings.Where(X => X.Label == "PhoneNumber" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "PhoneNumber").FirstOrDefault().Value : "",
                    CMS_Version = settings.Where(X => X.Label == "CMS_Version" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "CMS_Version").FirstOrDefault().Value : "",
                    MultiLanguage = settings.Where(X => X.Label == "MultiLanguage" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "MultiLanguage").FirstOrDefault().Value.ToLower() : "false",
                    ImageCrop = settings.Where(X => X.Label == "ImageCrop" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "ImageCrop").FirstOrDefault().Value.ToLower() : "false",
                    ImageResize = settings.Where(X => X.Label == "ImageResize" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "ImageResize").FirstOrDefault().Value.ToLower() : "false",
                    MaxFotoSize = settings.Where(X => X.Label == "MaxFotoSize" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "MaxFotoSize").FirstOrDefault().Value : "",
                    MaxVideoSize = settings.Where(X => X.Label == "MaxVideoSize" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "MaxVideoSize").FirstOrDefault().Value : "",
                    MaxFileSize = settings.Where(X => X.Label == "MaxFileSize" && X.Value != null).Count() > 0 ? settings.Where(x => x.Label == "MaxFileSize").FirstOrDefault().Value : "",
                    DocumentUrlPath = settings.Where(X => X.Label == "DocumentUrlPath" && X.Value != null).Count() > 0 ? settings.Where(X => X.Label == "DocumentUrlPath").FirstOrDefault().Value : "",
                };
                return list;
            }
            return null;
        }

        public async Task<bool> UdapteSettings(SettingsDto model)
        {
            IList<string> properties = model.GetType().GetProperties()
                .Select(p => p.Name).ToList();
            if (properties.Count > 0)
            {
                foreach (var item in properties)
                {
                    var Setting = new Setting();
                    Setting = await _cmsContext.Settings.Where(x => x.Label.Contains(item)).FirstOrDefaultAsync();
                    if (Setting != null)
                    {
                        Setting.Value = model.GetType().GetProperty(item).GetValue(model).ToString();
                        _cmsContext.Settings.Update(Setting);
                    }
                    else
                    {
                        int? maxId = _cmsContext.Settings.Max(x => x.Id);
                        maxId = maxId == null ? 1 : maxId.Value + 1;
                        var entity = new Setting()
                        {
                            Id = (int)maxId,
                            Label = item,
                            Value = model.GetType().GetProperty(item).GetValue(model).ToString()
                        };
                        _cmsContext.Settings.Add(entity);
                    }
                    await _cmsContext.SaveChangesAsync();

                }
                return true;
            }
            return false;
        }

        public async Task<bool> UdapteExtraSettings(Setting setting)
        {

            if (setting != null) {
                _cmsContext.Settings.Update(setting);
                await _cmsContext.SaveChangesAsync();

                return true;
            }
                
            return false;
        }

        public int ReadSequance(SequenceType lloji)
        {
            var sekuenca = _cmsContext.Sequences.Where(x => x.Name == lloji.ToString()).FirstOrDefault();

            return sekuenca != null ? (int)sekuenca.Currval : 0;
        }


        public async Task<bool> UpdateSequence(SequenceType lloji, int Id)
        {
            var sequence = await _cmsContext.Sequences.Where(x => x.Name == lloji.ToString()).FirstOrDefaultAsync();
            if (sequence != null)
            {
                sequence.Currval = Id + 1;
                _cmsContext.Sequences.Update(sequence);
                await _cmsContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
