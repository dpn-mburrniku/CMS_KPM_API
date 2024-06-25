using Contracts.IServices;
using System.Threading.Tasks;
using Entities;
using Entities.Models;
using CMS.API;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Entities.DataTransferObjects;

namespace Repository.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private CmsContext _cmsContext;
		public IUserRespository Users { get; private set; }
		public IRoleRepository Roles { get; private set; }
		public IBaseRepository BaseConfig { get; private set; }
		public ILayoutRepository Layouts { get; private set; }
		public ISysMenuRepository SysMenus { get; private set; }
		public ISocialNetworkRepository SocialNetwork { get; private set; }
		public IPageRepository Pages { get; private set; }
		public ILinkRepository Links { get; private set; }
		public IPostRepository Posts { get; private set; }
		public IPostCategoryRepository PostCategories { get; private set; }
		public IPostsInCategoryRepository PostsInCategory { get; private set; }
        public ITagInPostRepository PostsInTags { get; private set; }
        public ILiveChatRepository LiveChats { get; private set; }
		public IContactRepository Contacts { get; private set; }
		public ILinkTypeRepository LinkTypes { get; private set; }
		public IMediaRepository Media { get; private set; }
		public IPersonelRepository Personeli { get; private set; }
		public ISlideRepository Slide { get; private set; }
		public IMenuRepository Menu { get; private set; }
		public IContactMessagesRepository ContactMessages { get; private set; }
		public IFaqHeaderRepository FaqHeaders { get; private set; }
		public IFaqDetailRepository FaqDetails { get; private set; }
		public IGaleryHeaderRepository GaleryHeaders { get; private set; }
		public IGaleryCategoryRepository GaleryCategories { get; private set; }
		public IGaleryDetailRepository GaleryDetails { get; private set; }
		public IPostMediaRepository PostMedia { get; private set; }
		public IMenuTypeRepository MenuType { get; private set; }
		public IPageMediaRepository PageMedia { get; private set; }
		public ITagRepository Tags { get; private set; }
		public ISearchRepository Search { get; private set; }
        public ILogRepository Logs { get; private set; }
        public IUserActivityRepository UserActivity { get; private set; }
        public IMunicipalityLocationRepository MunicipalityLocations { get; private set; }
		public IMunicipalityRepository Municipality { get; private set; }
		public IMeasureUnitRepository MeasureUnits { get; private set; }
		public IEmailTemplateRepository EmailTemplates { get; private set; }
        public IEmailTemplateItemRepository EmailTemplateItem { get; private set; }
        public ILanguageRepository Language { get; private set; }
        public IResourceTranslationRepository ResourceTranslation { get; private set; }

        public UnitOfWork(CmsContext cmsContext
				, IHttpContextAccessor httpContextAccessor
				, IConfiguration configuration
				, UserManager<ApplicationUser> userManager
				, RoleManager<ApplicationRole> roleManager
				, IBaseRepository baseRepository
				)
		{
			Users = new UserRepository(cmsContext, userManager, baseRepository);
			Roles = new RoleRepository(cmsContext, roleManager);
			BaseConfig = new BaseRepository(cmsContext, httpContextAccessor, userManager, roleManager);
			Layouts = new LayoutRepository(cmsContext);
			SysMenus = new SysMenuRepository(cmsContext);
			SocialNetwork = new SocialNetworkRepository(cmsContext);
			Pages = new PageRepository(cmsContext);
			Links = new LinkRepository(cmsContext);
			LinkTypes = new LinkTypeRepository(cmsContext);
			Posts = new PostRepository(cmsContext);
			PostCategories = new PostCategoryRepository(cmsContext);
			PostsInCategory = new PostsInCategoryRepository(cmsContext);
			PostsInTags = new PostsInTagsRepository(cmsContext);
			LiveChats = new LiveChatRepository(cmsContext);
			Media = new MediaRepository(cmsContext);
			Contacts = new ContactRepository(cmsContext);
			Personeli = new PersonelRepository(cmsContext);
			Slide = new SlideRepostitory(cmsContext);
			Menu = new MenuRepository(cmsContext);
			ContactMessages = new ContactMessagesRepository(cmsContext);
			FaqHeaders = new FaqHeaderRepository(cmsContext);
			FaqDetails = new FaqDetailRepository(cmsContext);
			GaleryHeaders = new GaleryHeaderRepository(cmsContext);
			GaleryCategories = new GaleryCategoryRepository(cmsContext);
			GaleryDetails = new GaleryDetailRepository(cmsContext);
			PostMedia = new PostMediaRepository(cmsContext);
			MenuType = new MenuTypeRepository(cmsContext);
			PageMedia = new PageMediaRepository(cmsContext);
			Tags = new TagRepository(cmsContext);
			Search = new SearchRepository(cmsContext);
            Logs = new LogRepository(cmsContext);
            UserActivity = new UserActivityRepository(cmsContext);
            MunicipalityLocations = new MunicipalityLocationRepository(cmsContext);
			Municipality = new MunicipalityRepository(cmsContext);
			MeasureUnits = new MeasureUnitRepository(cmsContext);
			EmailTemplates = new EmailTemplateRepository(cmsContext);
            EmailTemplateItem = new EmailTemplateItemRepository(cmsContext);  
			Language = new LanguageRepository(cmsContext);
            ResourceTranslation = new ResourceTranslationRepository(cmsContext);
        }
		public async Task SaveAsync() => await _cmsContext.SaveChangesAsync();
	}
}
