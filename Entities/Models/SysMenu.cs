using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class SysMenu
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string NameSq { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string NameSr { get; set; } = null!;

    public string? Path { get; set; }

    public string? Icon { get; set; }

    public int? Type { get; set; }

    public int? OrderNo { get; set; }

    public bool? Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<SysMenu> InverseParent { get; } = new List<SysMenu>();

    public virtual SysMenu? Parent { get; set; }

    public virtual ICollection<SysMenuRole> SysMenuRoles { get; } = new List<SysMenuRole>();
}
