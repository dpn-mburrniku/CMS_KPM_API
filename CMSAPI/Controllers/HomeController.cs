using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Repositories;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }       

        public HomeController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper                    
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;          
        }

        [Authorize]
        [HttpGet]
        [Route("GetThemeConfig")]
        public async Task<IActionResult> GetThemeConfig()
        {
            var userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
            var themeconfig = await _unitOfWork.BaseConfig.GetThemeConfig(userLanguageId);

            return Ok(new
            {
                theme = themeconfig
            });

            //return Ok(themeconfig);
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateThemeConfig")]
        public async Task<IActionResult> UpdateThemeConfig(ThemeConfigDto model)
        {            
            var result = await _unitOfWork.BaseConfig.UpdateThemeConfig(model);
            if (result)
                return Ok();
            return BadRequest();
        }

        [Authorize]
        [HttpGet]
        [Route("GetSysMenu")]
        public async Task<IActionResult> GetSysMenu()
        {
            var userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
            var sysMenuList = await _unitOfWork.SysMenus.GetSysMenu(userLanguageId);
            return Ok(sysMenuList);
        }

        [Authorize]
        [HttpGet]
        [Route("GetComponentLocation")]
        public async Task<IActionResult> GetComponentLocation()
        {
            var SocialNetworkList = await _unitOfWork.BaseConfig.GetComponentLocation();
            return Ok(SocialNetworkList);
        }

        [Authorize]
        [HttpGet]
        [Route("GetLinkTypes")]
        public async Task<IActionResult> GetLinkTypes()
        {
            var linkTypes = await _unitOfWork.BaseConfig.GetLinkTypes();
            return Ok(linkTypes);
        }

        [Authorize]
        [HttpGet]
        [Route("GetPostCategories")]
        public async Task<IActionResult> GetPostCategories()
        {
            var postCategories = await _unitOfWork.BaseConfig.GetPostCategories();
            return Ok(postCategories);
        }

        [Authorize]
        [HttpGet]
        [Route("GetTemplates")]
        public async Task<IActionResult> GetTemplates()
        {
            var tempalates = await _unitOfWork.BaseConfig.GetTemplates();
            return Ok(tempalates);
        }

        //[Authorize]
        //[HttpGet]
        //[Route("GetPages")]
        //public async Task<IActionResult> GetPages()
        //{
        //    int userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
        //    var data = await _unitOfWork.Pages.FindByCondition(x => x.LanguageId == userLanguageId, false, null).ToListAsync();
        //    if (data.Count > 0)
        //    {
        //        var dataDto = _mapper.Map<IEnumerable<PageListDto>?>(data);
        //        return Ok(dataDto);
        //    }

        //    return NotFound();
        //}

        [Authorize]
        [HttpGet]
        [Route("GetMediaExCategory")]
        public async Task<IActionResult> GetMediaExCategory()
        {
            var data =  await _unitOfWork.BaseConfig.GetMediaExCategory();
            if (data.Count > 0)
            {                
                return Ok(data);
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        [Route("GetContact")]
        public async Task<IActionResult> GetContact()
        {
            var userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
            var data = await _unitOfWork.Contacts.FindByCondition(x => x.LanguageId == userLanguageId, false, null).ToListAsync();
            if (data.Count > 0)
            {
                var dataDto = _mapper.Map<IEnumerable<ContactDto>?>(data);
                return Ok(dataDto);
            }

            return NotFound();
        }


        
        [Authorize]
        [HttpGet]
        [Route("GetLangList")]
        public async Task<IActionResult> GetLangList()
        {
            var data = await _unitOfWork.BaseConfig.GetLangList();
            
           return Ok(data);           
        }

        [Authorize]
        [HttpGet]
        [Route("GetMultiLanguage")]
        public async Task<IActionResult> GetMultiLanguageSetting() 
        {
            var data = await _unitOfWork.BaseConfig.GetSysSettings();
            
            string multiLanguageSetting = data.Where(x => x.Label == "MultiLanguage").FirstOrDefault() != null ? data.Where(x => x.Label == "MultiLanguage").FirstOrDefault().Value : "true";
            bool status = bool.Parse(multiLanguageSetting.ToLower() != "false" ? "True" : "False");

            return Ok(status);
        }

        [Authorize]
        [HttpGet]
        [Route("GetSettings")]
        public async Task<IActionResult> GetSettings()
        {
            var data = await _unitOfWork.BaseConfig.GetSysSettings();            

            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        [Route("GetExtraSettings")]
        public async Task<IActionResult> GetExtraSettings()
        {
            var data = await _unitOfWork.BaseConfig.GetSysSettings();

            var filteredData = data.Where(x => x.Id > 21);
 
            return Ok(filteredData);
        }

        [Authorize]
        [HttpPost]
        [Route("GetExtraSettingsAsync")]
        public async Task<IActionResult> GetExtraSettingsAsync([FromBody] ExtraSettingsFilterParameters parameter)
        {
            var data = await _unitOfWork.BaseConfig.GetExtraSettingsAsync(parameter);
            var dataDTO = _mapper.Map<IEnumerable<ExtraSettingsDto>?>(data);

            return Ok(new
            {
                data = dataDTO,
                total = data.MetaData.TotalCount
            });
        }


        [Authorize]
        [HttpGet]
        [Route("GetSettingsForEdit")]
        public async Task<IActionResult> GetSettingsForEdit()
        {
            var data = await _unitOfWork.BaseConfig.GetSettingsForEdit();
            if (data != null) { return Ok(data); }
            return NotFound();
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateSettings")]
        public async Task<IActionResult> UpdateSettings(SettingsDto model)
        {
            var data = await _unitOfWork.BaseConfig.UdapteSettings(model);
            if(data)
                return Ok(data);

            return BadRequest();
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateExtraSettings")]
        public async Task<IActionResult> UpdateExtraSettings(List<UpdateExtraSettingsDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var settings = await _unitOfWork.BaseConfig.GetSettingById(item.Id);

                    if (settings != null)
                    {
                        settings.Value = item.Value;
                        await _unitOfWork.BaseConfig.UdapteExtraSettings(settings);

                    }
                }
                return Ok();
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet]
        [Route("GetDashboardStatistic")]
        public async Task<IActionResult> GetDashboardStatistic(int webLangId = 1)
        {

            var PostsStatictic = await _unitOfWork.Posts.FindByCondition(t => t.Deleted != true && t.LanguageId == webLangId, false, null).CountAsync();
            var PagesStatictic = await _unitOfWork.Pages.FindByCondition(t => t.Deleted != true && t.LanguageId == webLangId, false, null).CountAsync();
            var UsersStatictic = await _unitOfWork.Users.FindByCondition(t => t.Active == true, false, null).CountAsync();

            return Ok(new
            {
                posts = new
                {
                    value = PostsStatictic,
                },
                pages = new
                {
                    value = PagesStatictic,
                },
                users = new
                {
                    value = UsersStatictic,
                }
            });
        }

        [HttpGet]
        [Route("GetDashboardStatisticCategory")]
        public async Task<IActionResult> GetDashboardStatisticCategory(int LayoutId = 1, int webLangId = 1)
        {

            var postInCategoriesData = await _unitOfWork.PostsInCategory
            .FindAll(false, null)
            .Where(x => x.LanguageId == webLangId && x.PostCategory.LayoutId == LayoutId && x.Post.Deleted != true && x.PostCategory.Active == true)
            .GroupBy(x => x.PostCategoryId)
            .Select(x => new
            {
                Title = x.FirstOrDefault().PostCategory.Title,
                Count = x.Count()
            })
            .ToListAsync();

            var labelsArray = postInCategoriesData.Select(x => x.Title).ToArray();
            var dataArray = postInCategoriesData.Select(x => x.Count).ToArray();

            return Ok(new
            {
                labels = labelsArray,
                data = dataArray,
            });
        }

        [HttpGet]
        [Route("GetDashboardStatisticForMonth")]
        public Task<IActionResult> GetDashboardStatisticForMonth(int webLangId = 1)
        {
            var currentYear = DateTime.Now.Year;
            var lastYear = currentYear - 1;

            var vitiAktual = GetMonthlyCounts(_unitOfWork.Posts.FindAll(false, null), webLangId, currentYear);
            var vitiParaprak = GetMonthlyCounts(_unitOfWork.Posts.FindAll(false, null), webLangId, lastYear);

            return Task.FromResult<IActionResult>(Ok(new
            {
                vitiAktualArray = vitiAktual,
                vitiParaprakArray = vitiParaprak,
            }));
        }

        private int[] GetMonthlyCounts(IQueryable<Post> posts, int webLangId, int year)
        {
            var counts = new int[12];

            for (int month = 1; month <= 12; month++)
            {
                counts[month - 1] = posts
                    .Where(x => x.LanguageId == webLangId && x.Deleted != true && x.StartDate.Year == year && x.StartDate.Month == month)
                    .Count();
            }

            return counts;
        }

    }
}
