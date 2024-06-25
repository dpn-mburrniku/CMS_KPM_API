using Entities.Models;
using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class MunicipalityDto
    {
        public int Id { get; set; }       

        public string? NameSq { get; set; }

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }

        public bool? Active { get; set; }

    }

    public class UpdateMunicipalityDto
    {
        [Required]
        public int Id { get; set; }

        public string? NameSq { get; set; }

        public string? NameEn { get; set; } = string.Empty;

        public string? NameSr { get; set; } = string.Empty;

        public bool? Active { get; set; }

    }
}