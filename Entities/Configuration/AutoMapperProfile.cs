using Entities.Models;
using AutoMapper;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.WebDTOs;

namespace Entities.Configuration
{
	internal class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<AspNetUser, UserDto>().ReverseMap();
			CreateMap<ApplicationUser, UserDto>().ReverseMap();
			CreateMap<AspNetRole, RolesDto>().ReverseMap();
			CreateMap<ApplicationRole, RolesDto>().ReverseMap();
			CreateMap<AddLayout, Layout>().ReverseMap();
			CreateMap<UpdateLayout, Layout>().ReverseMap();
			CreateMap<LayoutDto, Layout>().ReverseMap();
			CreateMap<TemplateDto, Template>().ReverseMap();
			CreateMap<RoleLayoutsDto, LayoutRole>().ReverseMap();
			CreateMap<AddLayoutInRole, LayoutRole>().ReverseMap();
			CreateMap<AddMenuInRole, SysMenuRole>().ReverseMap();
			CreateMap<AddMenuDto, SysMenu>().ReverseMap();
			CreateMap<SysMenuDto, SysMenu>().ReverseMap();
			CreateMap<SysMenuListDto, SysMenu>().ReverseMap();
			CreateMap<RolesDtoAsync, AspNetRole>().ReverseMap();
			CreateMap<SocialNetworkDto, SocialNetwork>().ReverseMap();
			CreateMap<SocialNetworkListDto, SocialNetwork>().ReverseMap();
            CreateMap<UpdateSocialNetworkListDto, SocialNetwork>().ReverseMap();
            CreateMap<PageDto, Page>().ReverseMap();
			CreateMap<PageListDto, Page>().ForMember(dest => dest.InversePageNavigation, opt => opt.MapFrom(src => src.SubPages)).ReverseMap();
			CreateMap<PageJoinDto, Page>().ReverseMap();
			CreateMap<LinkDto, Link>().ReverseMap();
			CreateMap<LinkListDto, Link>().ReverseMap();
			CreateMap<LinkTypeDto, LinkType>().ReverseMap();
			CreateMap<LinkTypeListDto, LinkType>().ReverseMap();
			CreateMap<ComponentLocationDto, ComponentLocation>().ReverseMap();
			CreateMap<LiveChatDto, LiveChat>().ReverseMap();
			CreateMap<UpdateLiveChatDto, LiveChat>().ReverseMap();
			CreateMap<LiveChatListDto, LiveChat>().ForMember(dest => dest.InverseLiveChatNavigation, opt => opt.MapFrom(src => src.Children)).ReverseMap();
			CreateMap<PostDto, Post>().ReverseMap();
			CreateMap<PostUpdate, Post>().ReverseMap();
			CreateMap<PostListDto, Post>().ReverseMap();
			CreateMap<PostsInCategoryDto, PostsInCategory>().ReverseMap();
            CreateMap<TagsDto, PostsInTag>().ReverseMap();
			CreateMap<PostsInTagDto, PostsInTag>().ReverseMap();
            CreateMap<MediaDto, Medium>().ReverseMap();
			CreateMap<MediaListDto, Medium>().ReverseMap();
			CreateMap<MediaOtherSourceDto, Medium>().ReverseMap();
			CreateMap<ContactDto, Contact>().ReverseMap();
			CreateMap<ContactListDto, Contact>().ReverseMap();
            CreateMap<UpdateContactListDto, Contact>().ReverseMap();
            CreateMap<PersonelDto, Personel>().ReverseMap();
			CreateMap<PersonelListDto, Personel>().ReverseMap();
            CreateMap<UpdateOrderPersonelDto, Personel>().ReverseMap();
            CreateMap<MediaExCategoryDto, MediaExCategory>().ReverseMap();
			CreateMap<MediaExCategoryDto, MediaExCategory>().ReverseMap();
			CreateMap<MediaExCategoryDto, MediaExCategory>().ReverseMap();
			CreateMap<GenderDto, Gender>().ReverseMap();
			CreateMap<PostCategoryDto, PostCategory>().ReverseMap();
			CreateMap<PostCategoryListDto, PostCategory>().ReverseMap();
			CreateMap<SlideListDto, Slide>().ReverseMap();
			CreateMap<SlideDto, Slide>().ReverseMap();
			CreateMap<getSlideDto, Slide>().ReverseMap();
			CreateMap<UpdateSlideDto, Slide>().ReverseMap();
            CreateMap<LogsDto, Log>().ReverseMap();
            CreateMap<UserActivitiesDto, UserAudit>().ReverseMap();
            CreateMap<MediaExDto, MediaEx>().ReverseMap();
			CreateMap<PersonelGetByIdDto, Personel>().ReverseMap();
			CreateMap<SlideOtherSourceDto, Medium>().ReverseMap();
			CreateMap<SlideOtherSourceDto, Slide>().ReverseMap();
            CreateMap<UpdateSlideOrderDto, Slide>().ReverseMap();
            CreateMap<ContactMessagesDto, ContactMessage>().ReverseMap();
			CreateMap<ContactMessagesListDto, ContactMessage>().ReverseMap();
			CreateMap<UpdateContactMessagesDto, ContactMessage>().ReverseMap();
			CreateMap<FaqHeaderDto, Faqheader>().ReverseMap();
			CreateMap<FaqHeaderListDto, Faqheader>().ReverseMap();
			CreateMap<FaqDetailDto, Faqdetail>().ReverseMap();
			CreateMap<FaqDetailListDto, Faqdetail>().ReverseMap();
			CreateMap<GaleryHeaderListDto, GaleryHeader>().ReverseMap();
            CreateMap<UpdateGaleryHeaderOrderDto, GaleryHeader>().ReverseMap();
            CreateMap<GaleryHeaderDto, GaleryHeader>().ReverseMap();
			CreateMap<GaleryCategoryListDto, GaleryCategory>().ReverseMap();
			CreateMap<GaleryDetailDto, GaleryDetail>().ReverseMap();
			CreateMap<GaleryDetailListDto, GaleryDetail>().ReverseMap();
			CreateMap<MenuDto, Menu>().ReverseMap();
			CreateMap<UpdateMenuDto, Menu>().ReverseMap();
			CreateMap<MenuListDto, Menu>().ForMember(dest => dest.InverseMenuNavigation, opt => opt.MapFrom(src => src.Children)).ReverseMap();
			CreateMap<MenuOtherSourceDto, Menu>().ReverseMap();
			CreateMap<MenuTypeDto, MenuType>().ReverseMap();
			CreateMap<UpdateMenuTypeDto, MenuType>().ReverseMap();
			CreateMap<TagsDto, PostTag>().ReverseMap();
			CreateMap<TagsListDto, PostTag>().ReverseMap();
			CreateMap<UpdateMediaInPage, PageMedium>().ReverseMap();
			CreateMap<UpdateMunicipalityDto, Municipality>().ReverseMap();
			CreateMap<GaleryHeaderDto, GaleryHeader>().ReverseMap();
			CreateMap<GaleryCategoryListDto, GaleryCategory>().ReverseMap();
			CreateMap<GaleryDetailDto, GaleryDetail>().ReverseMap();
			CreateMap<GaleryDetailListDto, GaleryDetail>().ReverseMap();
			CreateMap<MenuDto, Menu>().ReverseMap();
			CreateMap<UpdateMenuDto, Menu>().ReverseMap();
			CreateMap<MenuListDto, Menu>().ForMember(dest => dest.InverseMenuNavigation, opt => opt.MapFrom(src => src.Children))
											.ForMember(dest => dest.Page1, opt => opt.MapFrom(src => src.PageParent)).ReverseMap();
			CreateMap<MenuOtherSourceDto, Menu>().ReverseMap();
			CreateMap<MenuTypeDto, MenuType>().ReverseMap();
			CreateMap<UpdateMenuTypeDto, MenuType>().ReverseMap();
			CreateMap<TagsDto, PostTag>().ReverseMap();
			CreateMap<TagsListDto, PostTag>().ReverseMap();
			CreateMap<UpdateMediaInPage, PageMedium>().ReverseMap();
			CreateMap<MunicipalityDto, Municipality>().ReverseMap();
			CreateMap<UpdateMunicipalityDto, Municipality>().ReverseMap();
			CreateMap<MeasureUnitDto, MeasureUnit>().ReverseMap();
			CreateMap<MunicipalityLocationDto, MunicipalityLocation>().ReverseMap();
			CreateMap<getMunicipalityLocationDto, MunicipalityLocation>().ReverseMap();
			CreateMap<MunicipalityLocationListDto, MunicipalityLocation>().ReverseMap();
			CreateMap<LocationCordinateListDto, MunicipalityLocation>().ReverseMap();
			CreateMap<CreatePostCategoryDto, PostCategory>().ReverseMap();
			CreateMap<EmailDto, ContactMessage>().ReverseMap();
			CreateMap<EmailTemplateDto, EmailTemplate>().ReverseMap();
			CreateMap<EmailTemplateListDto, EmailTemplate>().ReverseMap();
			CreateMap<EmailTemplateItemDto, EmailTemplateItem>().ReverseMap();
			CreateMap<EmailTemplateItemListDto, EmailTemplateItem>().ReverseMap();
            CreateMap<LanguageDto, Language>().ReverseMap();
            CreateMap<UpdateLanguageDto, Language>().ReverseMap();
            CreateMap<LanguageListDto, Language>().ReverseMap();
			CreateMap<ResourceTranslationStringListDto, ResourceTranslationString>().ReverseMap();
			CreateMap<ResourceTranslationTypeDto,  ResourceTranslationType>().ReverseMap();
			CreateMap<ResourceTranslationStringDto, ResourceTranslationString>().ReverseMap();
			CreateMap<ResourceTransStringsDto, ResourceTranslationString>().ReverseMap();
			CreateMap<ResourceTranslationTypeWithTransDto, ResourceTranslationType>()
				.ForMember(dest => dest.ResourceTranslationStrings, opt => opt.MapFrom(src => src.resourceTranslationString)).ReverseMap();
            CreateMap<UpdateExtraSettingsDto, Setting>().ReverseMap();
            CreateMap<ExtraSettingsDto, Setting>().ReverseMap();
            

            //Web auto mapper12
            //CreateMap<MenuModel, Menu>().ForMember(dest => dest.InverseMenuNavigation, opt => opt.MapFrom(src => src.submenu))
            //                            .ForMember(dest => dest.Page1, opt => opt.MapFrom(src => src.PageParent))
            //                            .ForMember(dest => dest.PageNavigation, opt => opt.MapFrom(src => src.PageRidirect)).ReverseMap();

            //CreateMap<MenuWithoutChildsModel, Menu>().ForMember(dest => dest.Page1, opt => opt.MapFrom(src => src.PageParent))
            //                            .ForMember(dest => dest.PageNavigation, opt => opt.MapFrom(src => src.PageRidirect)).ReverseMap();

            //CreateMap<PageModel, Page>().ForMember(dest => dest.InversePageNavigation, opt => opt.MapFrom(src => src.subPages)).ReverseMap();
            //CreateMap<SubPageModel, Page>().ReverseMap();
            //CreateMap<PageMenuModel, BasicPageModel>().ReverseMap();
            //CreateMap<BasicPageModel, Page>().ReverseMap();
            //CreateMap<PageMenuModel, Page>().ReverseMap();
            //CreateMap<PageMediaModel, PageMedium>().ReverseMap();
            //CreateMap<ContactModel, Contact>().ReverseMap();
            //CreateMap<LinksModel, Link>().ReverseMap();

            //CreateMap<LiveChatModel, LiveChat>().ForMember(dest => dest.InverseLiveChatNavigation, opt => opt.MapFrom(src => src.childs)).ReverseMap();
            //CreateMap<ChildLiveChatModel, LiveChat>().ReverseMap();
            //CreateMap<SlidesModel, Slide>().ReverseMap();
            //CreateMap<SocialNetworksModel, SocialNetwork>().ReverseMap();
            CreateMap<FAQModel, Faqheader>().ReverseMap();
			CreateMap<FAQDetailsModel, Faqdetail>().ReverseMap();
			//CreateMap<MediaModel, Medium>().ForMember(dest => dest.FileExNavigation, opt => opt.MapFrom(src => src.MediaEx)).ReverseMap();
			//CreateMap<MediaExModel, MediaEx>().ReverseMap();
			//CreateMap<GaleryModel, GaleryHeader>().ReverseMap();
			//CreateMap<GaleryDetailsModel, GaleryDetail>().ReverseMap();
			//CreateMap<NewsInCategoriesModel, PostsInCategory>().ReverseMap();
			//CreateMap<NewsModel, Post>().ReverseMap();
			//CreateMap<PostCategoryModel, PostCategory>().ReverseMap();
			//CreateMap<NewsMediaModel, PostMedium>().ReverseMap();
			//CreateMap<NewsCategoriesModel, PostCategory>().ReverseMap();
			//CreateMap<PersonelModel, Personel>().ReverseMap();
		}
	}
}

