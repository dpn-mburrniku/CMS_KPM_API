using CMS.API.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CreateUserModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string UserName { get;set; }

        [Required]
        public string Email { get; set; }
        public string? PersonalNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } 
        public string? PhoneNumber { get; set; }
        public LanguageEnum Language { get; set; }        

        public string? Birthdate { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? WorkPosition { get; set; }

        public bool Active { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }        
        public bool Reset { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
    public class EditUserModel
    {
        public string ID { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        public string? PersonalNumber { get; set; }

        public string? CurrentPassword { get; set; }

        public string? Password { get; set; }
        [Required]
        public string Role { get; set; }
        public string? PhoneNumber { get; set; }
        public LanguageEnum Language { get; set; }

        public string? Birthdate { get; set; }
        public GenderEnum Gender { get; set; }
        public string? WorkPosition { get; set; }

        public bool Active { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
        public bool Reset { get; set; }
        public IFormFile? ProfileImage { get;set;}
    }

    //public class role 
    //{
    //    [JsonPropertyName("value")]
    //    public string Id { get; set; }
    //}

    public class ResetPasswordFromAdmin
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string NewPassword { get; set; }    
    }

    public class ResetPassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
