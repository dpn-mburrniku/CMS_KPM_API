using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class TemplateDto
    {
        public int Id { get; set; }

        public string? TemplateName { get; set; }

        public string? TemplateUrl { get; set; }

        public bool? TemplateUrlWithId { get; set; }
    }
}
