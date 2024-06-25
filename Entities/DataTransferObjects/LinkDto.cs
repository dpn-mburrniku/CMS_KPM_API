using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LinkDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }
        public bool WebMultiLang { get; set; }

        public int? PageId { get; set; }

        public int? LayoutId { get; set; }

        public int TypeId { get; set; }

        public string LinkName { get; set; }

        public string Url { get; set; }

        public string LinkTarget { get; set; }

        public bool? Active { get; set; }

        public int OrderNo { get; set; }


    }

    public class LinkListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int? PageId { get; set; }

        public int? LayoutId { get; set; }

        public int TypeId { get; set; }

        public string LinkName { get; set; }

        public string Url { get; set; }

        public string LinkTarget { get; set; }

        public bool? Active { get; set; }
        public int OrderNo { get; set; }


        public virtual LayoutDto Layout { get; set; } = null!;

        public virtual LinkTypeDto LinkType { get; set; } = null!;

        public virtual PageDto? Page { get; set; }
    }

    public class UpdateLinkListDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }
}
