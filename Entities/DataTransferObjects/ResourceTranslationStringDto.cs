using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ResourceTranslationStringDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }

        public int TypeId { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }
    }
    public class ResourceTranslationStringListDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }

        public int TypeId { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }
        public virtual ResourceTranslationTypeDto Type { get; set; } = null!;
    }

}
