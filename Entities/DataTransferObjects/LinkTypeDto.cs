using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LinkTypeDto
    {
        public int LinkTypeId { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public string? LinkuTypeName { get; set; }

        public int ComponentLocationId { get; set; }


    }
    public class LinkTypeListDto
    {
        public int LinkTypeId { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public string? LinkuTypeName { get; set; }

        public int ComponentLocationId { get; set; }

        public virtual ComponentLocationDto ComponentLocation { get; set; } = null!;
    }
}
