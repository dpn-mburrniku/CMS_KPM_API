using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class GenderDto
    {
            public int Id { get; set; }

            public int LanguageId { get; set; }

            public string? Name { get; set; }

        
    }
}
