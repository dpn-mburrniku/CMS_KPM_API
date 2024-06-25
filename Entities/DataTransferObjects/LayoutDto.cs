using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LayoutDto
    {
        public int Id { get; set; }
        public string? NameSq { get; set; }
        public string? NameEn { get; set; } = string.Empty;
        public string? NameSr { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
    }

    public class AddLayout
    {
        [Required]
        public string? NameSq { get; set; }
        public string? NameEn { get; set; }
        public string? NameSr { get; set; }
        public string? Description { get; set; }
        public string? Path { get; set; }
    }

    public class UpdateLayout
    {
        public int Id { get; set; }
        [Required]
        public string? NameSq { get; set; }
        public string? NameEn { get; set; } = string.Empty;
        public string? NameSr { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
    }

    public class AddLayoutInRole
    {
        [Required]
        public string RoleId { get; set; }
        [Required]
        public int LayoutId { get; set; }
    }
    public class LayoutsForRoleDto
    {
        public int Id { get; set; }
        public int LayoutId { get; set; }
        [Required]
        public string Name_SQ { get; set; }
        public string Name_EN { get; set; } = string.Empty;
        public string Name_SR { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }

    public class RoleLayoutsDto
    {
        public int Id { get; set; }

        public string RoleId { get; set; } = null!;

        public int LayoutId { get; set; }

        public virtual LayoutDto Layout { get; set; } = null!;
    }

}
