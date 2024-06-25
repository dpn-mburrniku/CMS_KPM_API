using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ThemeConfigDto
    {        
            public int Id { get; set; }

            public string? UserId { get; set; }

            public string? ThemeColor { get; set; }

            public string? Mode { get; set; }

            public int? PrimaryColorLevel { get; set; }

            public string? NavMode { get; set; }
            public int Locale { get; set; }

            public string? LayoutType { get; set; }
    }
}
