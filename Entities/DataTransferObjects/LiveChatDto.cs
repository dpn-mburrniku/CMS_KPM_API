using Entities.Models;
using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.DataTransferObjects
{
	public class LiveChatDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int? PageId { get; set; }

		public int? Level { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public bool IsOtherSource { get; set; }

		public string? OtherSourceName { get; set; }

		public string? OtherSource { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }
	}

	public class UpdateLiveChatDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int? PageId { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public bool IsOtherSource { get; set; }

		public string? OtherSourceName { get; set; }

		public string? OtherSource { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }
	}

	public class LiveChatListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int? PageId { get; set; }

		public int? ParentId { get; set; }

		public int? Level { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

		public bool IsOtherSource { get; set; }

		public string? OtherSourceName { get; set; }

		public string? OtherSource { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }

		public virtual PageJoinDto? Page { get; set; }

		public virtual List<LiveChatListDto>? Children { get; set; }
	}
}
