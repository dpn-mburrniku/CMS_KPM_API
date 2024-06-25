using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        [StringLength(128)]
        public string Name_SQ { get; set; }

        [Required]
        [StringLength(128)]
        public string Name_EN { get; set; }

        [Required]
        [StringLength(128)]
        public string Name_SR { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }      
    }
}
