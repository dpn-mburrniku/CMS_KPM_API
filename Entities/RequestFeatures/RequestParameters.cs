namespace Entities.RequestFeatures
{
	public abstract class RequestParameters
	{
		const int maxPageSize = 100;
		public int _pageIndex = 1;
		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = value == 0 ? 1 : value;
			}
		}
		private int _pageSize = 10;
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				//_pageSize = (value > maxPageSize) ? maxPageSize : value;
				_pageSize = value;
			}
		}
		public int? webLangId { get; set; } = 1;
		public int? LayoutId { get; set; }
		public Sort Sort { get; set; }
	}

	public class FilterParameters : RequestParameters
	{
		public FilterParameters()
		{
		}
		public string? Query { get; set; }
	}

	public class GaleryFilterParameters : RequestParameters
	{
		public GaleryFilterParameters()
		{
		}
		public int GaleryCategoryId { get; set; }
		public string? Query { get; set; }
	}
	public class GaleryDetailParameters : RequestParameters
	{
		public GaleryDetailParameters()
		{
		}
		public int GaleryMediaId { get; set; }
		public int GaleryHeaderId { get; set; }
		public string? Query { get; set; }
	}

    public class MunicipalityLocationParameters : RequestParameters
	{
		public MunicipalityLocationParameters()
		{
		}
		public int MeasureUnitId { get; set; }
		public int MunicipalityId { get; set; }
		public string? Query { get; set; }
	}

	public class TreeTypesForLocationParameters : RequestParameters
	{
		public TreeTypesForLocationParameters()
		{
		}
		public int MunicipalityLocationId { get; set; }
		public string? Query { get; set; }
	}

	public class CertificateTypeConfigurtationParameters : RequestParameters
	{
		public CertificateTypeConfigurtationParameters()
		{
		}
		public int CertificateTypeId { get; set; }
		public string? Query { get; set; }
	}

	public class PaymentParameters : RequestParameters
	{
		public PaymentParameters()
		{
		}
		public bool? Succeed { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? Query { get; set; }
	}

	public class TreeTypeConfigurtationParameters : RequestParameters
	{
		public TreeTypeConfigurtationParameters()
		{
		}
		public int TreeTypeId { get; set; }
		public string? Query { get; set; }
	}
    public class ResourceTranslationStringFilterParameters : RequestParameters
    {
        public ResourceTranslationStringFilterParameters()
        {
        }
        public int ResourceTranslationTypeId { get; set; }
        public string? Query { get; set; }
    }

    public class MailStringFilterParameters : RequestParameters
    {
        public MailStringFilterParameters()
        {
        }
        public int PageId { get; set; }
        public int ContactId { get; set; }
        public bool? IsDraft { get; set; }
        public bool? IsFavorite { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public string? Query { get; set; }
    }

    public class LinkFilterParameters : RequestParameters
	{
		public LinkFilterParameters()
		{
		}
		public int LinkTypeId { get; set; }
		public string? Query { get; set; }
	}

	public class LinkTypeFilterParameters : RequestParameters
	{
		public LinkTypeFilterParameters()
		{
		}
		public int ComponentLocationId { get; set; }
		public string? Query { get; set; }
	}
	public class SocialNetworkFilterParameters : RequestParameters
	{
		public SocialNetworkFilterParameters()
		{
		}
		public int ComponentLocationId { get; set; }
		public string? Query { get; set; }
	}

	public class PageFilterParameters : RequestParameters
	{
		public PageFilterParameters()
		{
		}
		public int? TemplateId { get; set; }
		public int? parentPageId { get; set; } = null;
		public string? Query { get; set; }
		public bool isDeleted { get; set; } = false;
	}
	public class SubPageFilterParameters : RequestParameters
	{
		public SubPageFilterParameters()
		{
		}
		public int? TemplateId { get; set; }
		public int? SubPageParentId { get; set; } = null;
		public string? Query { get; set; }
		public bool isDeleted { get; set; } = false;
	}
	public class MediaFilterParameters : RequestParameters
	{
		public MediaFilterParameters()
		{
		}
		public int? MediaExCategoryId { get; set; }
		public string? DateFrom { get; set; }
		public string? DateTo { get; set; }
		public string? Query { get; set; }
	}

	public class PostFilterParameters : RequestParameters
	{
		public PostFilterParameters()
		{
		}
		public int PostCategoryId { get; set; }
		public string? DateFrom { get; set; }
		public string? DateTo { get; set; }
		public string? Query { get; set; }
	}

	public class ContactFilterParameters : RequestParameters
	{
		public ContactFilterParameters()
		{
		}
		public int? PageId { get; set; }
		public string? Query { get; set; }
	}

    public class PersonelFilterParameters : RequestParameters
	{
		public PersonelFilterParameters()
		{
		}
		public int? PageId { get; set; }
		public string? Query { get; set; }
	}

	public class LiveChatFilterParameters : RequestParameters
	{
		public LiveChatFilterParameters()
		{
		}
		public int? PageId { get; set; }
		public string? Query { get; set; }
	}

	public class PostCategoryFilterParameters : RequestParameters
	{
		public PostCategoryFilterParameters()
		{
		}
		public int? PageId { get; set; }
		public string? Query { get; set; }
	}

	public class FaqHeaderParameters : RequestParameters
	{
		public FaqHeaderParameters()
		{
		}
		public int? PageId { get; set; }
		public string? Query { get; set; }
	}

    public class LogFilterParameters : RequestParameters
    {
        public LogFilterParameters()
        {
        }
        public bool IsError { get; set; } = false;
        public string? Username { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? Method { get; set; }
        public string? Controller { get; set; }
        public string? Query { get; set; }
    }

    public class UserAuditFilterParameters : RequestParameters
    {
        public UserAuditFilterParameters()
        {
        }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? UserId { get; set; }
        public string? Query { get; set; }
    }

    public class UserActivitiesFilterParameters : RequestParameters
    {
        public UserActivitiesFilterParameters()
        {
        }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? Username { get; set; }
        public int? ActionTypeId { get; set; }
    }

    public class FaqDetailParameters : RequestParameters
	{
		public FaqDetailParameters()
		{
		}
		public int? FaqHeaderId { get; set; }
		public string? Query { get; set; }
	}

    public class EmailItemsFilterParameters : RequestParameters
    {
        public EmailItemsFilterParameters()
        {
        }
        public int EmailTemplateId { get; set; }
        public string? Query { get; set; }
    }

    public class LanguageParameters : RequestParameters
    {
        public LanguageParameters()
        {
        }
        public int CultureCodeId { get; set; }
        public string? Query { get; set; }
    }


    public class CertificateParameters : RequestParameters
    {
        public CertificateParameters()
        {
        }
		public int? MunicipalityId { get; set; }
		public int? LocationId { get; set; }
		public int? TreeTypeId { get; set; }
		public int? CertificateTypeId { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? Query { get; set; }
    }

    public class Sort
	{
		public string order { get; set; }
		public string key { get; set; }
	}
    public class ExtraSettingsFilterParameters : RequestParameters
    {
        public ExtraSettingsFilterParameters()
        {
        }
    }
}
