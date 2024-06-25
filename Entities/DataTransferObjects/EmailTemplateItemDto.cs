using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class EmailTemplateItemDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int EmailTemplateId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;
    }

    public class EmailTemplateItemListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int EmailTemplateId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;

        public virtual EmailTemplate EmailTemplate { get; set; } = null!;

    }

}
