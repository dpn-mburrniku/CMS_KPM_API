using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LanguageDto
    {

        public int Id { get; set; }

        public int? CultureCodeId { get; set; }

        public string NameSq { get; set; } = null!;

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }

        public bool? Active { get; set; }

    }

    public class UpdateLanguageDto
    {
        [Required]
        public int Id { get; set; }

        public int? CultureCodeId { get; set; }

        public string NameSq { get; set; } = null!;

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }

        public bool? Active { get; set; }

    }

    public class LanguageListDto
    {
        [Required]
        public int Id { get; set; }

        public int? CultureCodeId { get; set; }

        public string NameSq { get; set; } = null!;

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }

        public bool? Active { get; set; }

        public virtual CultureCode? CultureCode { get; set; }

    }
}
