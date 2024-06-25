using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class EmailTemplateDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public string Name { get; set; } = null!;

		public string Subject { get; set; } = null!;

		public string Content { get; set; } = null!;
	}

	public class EmailTemplateListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public string Name { get; set; } = null!;

		public string Subject { get; set; } = null!;

		public string Content { get; set; } = null!;
		public virtual ICollection<EmailTemplateItem> EmailTemplateItems { get; set; } = new List<EmailTemplateItem>();

	}
}
