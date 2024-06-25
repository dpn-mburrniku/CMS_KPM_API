using CMS.API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class GetProfileDto
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string PersonalNumber { get; set; }
        public string PhoneNumber { get; set; }
        public LanguageEnum Language { get; set; }
        public string Birthdate { get; set; }
        public GenderEnum Gender { get; set; }
        public string? WorkPosition { get; set; }
        public string? ProfileImage { get; set; }
    }
    public class ProfileDto
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }
        public string? PersonalNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public LanguageEnum Language { get; set; }
        public string? Birthdate { get; set; }
        public GenderEnum Gender { get; set; }
        public string? WorkPosition { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

    public class ProfileImage
    {
        public string UserId { get; set; }
    }
}
