using Entities.Models;
using Entities.DataTransferObjects;
using CMS.API.Helpers;
using Entities.RequestFeatures;

namespace Contracts.IServices
{
    public interface IBaseRepository
    {
        string GetLoggedUsername();
        string GetLoggedUserId(); 
        int GetCurrentUserLanguage(); 
        DateTime StringToDate(string rdata);
        DateTime StringToDateTime(string rdata);
        int GetGjuhaID(string Gjuha);
        public int GetTotalPages(int totalRowsCount, int take);
        DateTime GetDateTime();
        int? GetLayoutID(string Layout);
        Task<List<string>> GetUserRoles(string userId);
        Task<List<string>> GetUserRolesId(string userId);
        Task<ThemeConfigDto> GetThemeConfig(int userLanguage);
        Task<bool> UpdateThemeConfig(ThemeConfigDto model);
        Task<List<ComponentLocation>> GetComponentLocation();
        Task<List<LinkType>> GetLinkTypes();
        Task<List<PostCategory>> GetPostCategories();
        Task<List<Template>> GetTemplates();
        Task<List<Language>> GetLangList();
        Task<List<MediaExCategory>> GetMediaExCategory();
        Task<List<Setting>> GetSysSettings();
        Task<PagedList<Setting>> GetExtraSettingsAsync(ExtraSettingsFilterParameters parameter);
        Task<Setting?> GetSettingById(int id);
        Task<SettingsDto> GetSettingsForEdit();
        Task<bool> UdapteSettings(SettingsDto model);
        Task<bool> UdapteExtraSettings(Setting setting);
        int ReadSequance(SequenceType lloji);
        Task<bool> UpdateSequence(SequenceType lloji, int Id);
    }
}
