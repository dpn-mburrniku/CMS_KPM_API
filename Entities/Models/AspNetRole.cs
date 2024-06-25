using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AspNetRole
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string NameSq { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string NameSr { get; set; } = null!;

    public string? Description { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<SysMenuRole> SysMenuRoles { get; set; } = new List<SysMenuRole>();

    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();
}
