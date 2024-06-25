using Entities.Models;
using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public partial class PostsInCategoryDto
	{
		public int PostCategoryId { get; set; }
		public virtual PostCategoryDto PostCategory { get; set; } = null!;
	}

	public partial class CreatePostCategoryDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }
		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public int? PageId { get; set; }

		public string Title { get; set; } = null!;

		public bool? Active { get; set; }

		public string? Extra { get; set; } = string.Empty!;

		public bool? ShowInFilters { get; set; }
	}


	public partial class PostCategoryDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }
		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public virtual LayoutDto Layout { get; set; } = null!;

		public int? PageId { get; set; }

		public string Title { get; set; } = null!;

		public string TitleWithLayout
		{
			get
			{
				return LayoutId > 1 ? (LanguageId == 2 ? Layout.NameEn + " - " : (LanguageId == 3 ? Layout.NameSr + " - " : Layout.NameSq + " - ")) + Title : Title;
			}
		}


		public bool? Active { get; set; }

		public string? Extra { get; set; }

		public bool? ShowInFilters { get; set; }
	}

	public partial class PostCategoryListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }
		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public int? PageId { get; set; }

		public string Title { get; set; } = null!;

		public string TitleWithLayoutName
		{
			get
			{
				return LayoutId > 1 ? (LanguageId == 2 ? Layout.NameEn + " - " : (LanguageId == 3 ? Layout.NameSr + " - " : Layout.NameSq + " - ")) + Title : Title;
			}
		}

		public bool? Active { get; set; }

		public string? Extra { get; set; }

		public bool? ShowInFilters { get; set; }

		public virtual LayoutDto Layout { get; set; } = null!;

		public virtual PageDto? Page { get; set; }
	}
}
