using System.Threading.Tasks;

namespace Contracts.IServices
{
	public interface IUnitOfWork
	{
		IUserRespository Users { get; }
		IRoleRepository Roles { get; }
		IBaseRepository BaseConfig { get; }
		ILayoutRepository Layouts { get; }
		ISysMenuRepository SysMenus { get; }
		ISocialNetworkRepository SocialNetwork { get; }
		IPageRepository Pages { get; }
		ILinkRepository Links { get; }
		IPostRepository Posts { get; }
		IPostCategoryRepository PostCategories { get; }
		IPostsInCategoryRepository PostsInCategory { get; }
        ITagInPostRepository PostsInTags { get; }
        ILiveChatRepository LiveChats { get; }
		IContactRepository Contacts { get; }
		ILinkTypeRepository LinkTypes { get; }
		IMediaRepository Media { get; }
		IPersonelRepository Personeli { get; }
		ISlideRepository Slide { get; }
		IMenuRepository Menu { get; }
		IContactMessagesRepository ContactMessages { get; }
		IFaqHeaderRepository FaqHeaders { get; }
		IFaqDetailRepository FaqDetails { get; }
		IGaleryHeaderRepository GaleryHeaders { get; }
		IGaleryCategoryRepository GaleryCategories { get; }
		IGaleryDetailRepository GaleryDetails { get; }
		IPostMediaRepository PostMedia { get; }
		IMenuTypeRepository MenuType { get; }
		IPageMediaRepository PageMedia { get; }
		ITagRepository Tags { get; }
		ISearchRepository Search { get; }
		IMunicipalityLocationRepository MunicipalityLocations { get; }
		IMeasureUnitRepository MeasureUnits { get; }
		IMunicipalityRepository Municipality { get; }
		IEmailTemplateRepository EmailTemplates { get; }
        IEmailTemplateItemRepository EmailTemplateItem { get; }
        ILogRepository Logs { get; }
        IUserActivityRepository UserActivity { get; }
		ILanguageRepository Language { get; }
		IResourceTranslationRepository ResourceTranslation { get; }
        Task SaveAsync();
	}
}
