using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SocialNetworkDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int LayoutId { get; set; }

        public int ComponentLocationId { get; set; }

        public string Name { get; set; } = null!;

        public string? Link { get; set; }

        public string? ImgPath { get; set; }

        public string? Html { get; set; }

        //public int? OrderNo { get; set; }

        public bool? Active { get; set; }

    }

    public class SocialNetworkListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int LayoutId { get; set; }

        public int ComponentLocationId { get; set; }

        public string Name { get; set; } = null!;

        public string? Link { get; set; }

        public string? ImgPath { get; set; }

        public string? Html { get; set; }

        public int? OrderNo { get; set; }

        public bool? Active { get; set; }

        public virtual ComponentLocationDto ComponentLocation { get; set; } = null!;

        public virtual LayoutDto Layout { get; set; } = null!;
    }

    public class UpdateSocialNetworkListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int? OrderNo { get; set; }

    }

}
