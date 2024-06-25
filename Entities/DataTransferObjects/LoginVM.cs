using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LoginVM
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Token { get; set; } = "";
    }

    public class rechapchaRespondeLogin
    {
        public bool success { get; set; }
        public string hostname { get; set; }
    }
}
