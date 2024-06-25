using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class SysMenuRole
{
    public int Id { get; set; }

    public int SysMenuId { get; set; }

    public string RoleId { get; set; } = null!;

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual AspNetRole Role { get; set; } = null!;

    public virtual SysMenu SysMenu { get; set; } = null!;
}
