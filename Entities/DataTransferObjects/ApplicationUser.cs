using CMS.API.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Language = LanguageEnum.Albania;          
            Created = DateTime.Now;
            Gender = GenderEnum.Mashkull;
        }
        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        
        [StringLength(10)]
        public string? PersonalNumber { get; set; }

        [MaxLength(512)]
        public string? ProfileImage { get; set; }

        public DateTime PasswordExpires { get; set; }

        public bool ChangePassword { get; set; }

        public DateTime Created { get; set; }

        public LanguageEnum Language { get; set; }          

        public DateTime? Birthdate { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? WorkPosition { get; set; }

        public bool Active { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? Modified { get; set; }
        public string? CreateBy { get; set; } 
        public string? ModifiedBy { get; set; }       

    }
}
