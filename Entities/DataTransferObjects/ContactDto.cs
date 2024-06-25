using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class ContactDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int LayoutId { get; set; }

        public int? PageId { get; set; }

        public int? MediaId { get; set; }

        public int? GenderId { get; set; }

        public string? Description { get; set; }

        public string? ContactPerson { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? Fax { get; set; }

        public string? Email { get; set; }

        public string? Longitude { get; set; }

        public string? Latitude { get; set; }

        public string? MapLocation { get; set; }
        //public int OrderNo { get; set; }
    }

    public class ContactListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int LayoutId { get; set; }

        public int? PageId { get; set; }

        public int? MediaId { get; set; }

        public int? GenderId { get; set; }

        public string? Description { get; set; }

        public string? ContactPerson { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }
        public string? PhoneNumber2 { get; set; }
        public string? Fax { get; set; }

        public string? Email { get; set; }

        public string? Longitude { get; set; }

        public string? Latitude { get; set; }

        public string? MapLocation { get; set; }
        public int OrderNo { get; set; }

        public virtual GenderDto? Gender { get; set; }

        public virtual LayoutDto Layout { get; set; } = null!;

        public virtual PageDto? Page { get; set; }
    }

    public class UpdateContactListDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }

}
