using Entities.Models; using CMS.API;
using CMS.API.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public partial class UserDto
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? PersonalNumber { get; set; } = null!;

        public string? ProfileImage { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Birthdate { get; set; }

        public int? Gender { get; set; }
        public string? WorkPosition { get; set; }          

        public int? Language { get; set; }        

        public DateTime PasswordExpires { get; set; }

        //public DateTime Created { get; set; }     
        public string Role { get; set; }       

        public bool? Active { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
        //public DateTime? Modified { get; set; }
        //public string? CreateBy { get; set; }
        //public string? ModifiedBy { get; set; }

    }
}
