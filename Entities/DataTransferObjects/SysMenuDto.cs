using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SysMenuDto
    {
        public int Key { get; set; }

        public int? ParentId { get; set; }

        public string Title { get; set; }

        public string? Path { get; set; }

        public string? Icon { get; set; }

        public int? Type { get; set; }

        public int? OrderNo { get; set; }

        public bool? Active { get; set; }

        public List<string> Authority { get; set; }

        public bool? HasSubMenu { get; set; }

        public List<SysMenuDto> SubMenu { get; set; }

    }

    public class SysMenuListDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string NameSQ { get; set; }
        [Required]
        public string NameEN { get; set; }
        [Required]
        public string NameSR { get; set; }
        public int Key { get; set; }

        public int? ParentId { get; set; }

        public string Title { get; set; }

        public string? Path { get; set; }

        public string? Icon { get; set; }

        public int? Type { get; set; }

        public int? OrderNo { get; set; }

        public bool? Active { get; set; }

        public List<string> Authority { get; set; }

        public bool? HasSubMenu { get; set; }

        public List<SysMenuDto> SubMenu { get; set; }

        public virtual SysMenu? Parent { get; set; }

    }

    public class AddMenuDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public string NameSQ { get; set; }
        [Required]
        public string NameEN { get; set; }
        [Required]
        public string NameSR { get; set; }
        public string Path { get; set; }
        public string? Icon { get; set; }
        [Required]
        public int Type { get; set; }
        public int? OrderNo { get; set;}
        public bool? Active { get; set; }

    }

    public class UpdateSysMenuListDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }
}
