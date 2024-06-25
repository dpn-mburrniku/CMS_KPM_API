using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Entities.DataTransferObjects
{

    public class ResourceTranslationTypeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class ResourceTranslationTypeByLangDto
    {
        public int LangId { get; set; }
        public string Lang { get; set; }

        public List<ResourceTranslationTypeWithTransDto> resourceTranslationType { get; set; }

    }
    public class ResourceTranslationTypeWithTransDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public List<ResourceTransStringsDto> resourceTranslationString { get; set; }
    }

    public class ResourceTransStringsDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }
    }
}
