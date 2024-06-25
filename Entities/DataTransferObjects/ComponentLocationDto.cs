using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public partial class ComponentLocationDto
    {
        public int Id { get; set; }

        public string TitleSq { get; set; } = null!;

        public string TitleEn { get; set; } = null!;

        public string TitleSr { get; set; } = null!;

        public int? OrderNo { get; set; }

        public bool? Active { get; set; }
    }
}
