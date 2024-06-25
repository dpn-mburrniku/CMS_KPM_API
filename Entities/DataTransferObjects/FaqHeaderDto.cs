using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class FaqHeaderDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int LayoutId { get; set; }

        public int? PageId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

    }
    public class FaqHeaderListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int LayoutId { get; set; }

        public int? PageId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public virtual LayoutDto Layout { get; set; } = null!;

        public virtual PageDto? Page { get; set; }


    }
}
