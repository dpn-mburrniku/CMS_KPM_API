using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? PersonalNumber { get; set; }

    public string? ProfileImage { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateTime? Birthdate { get; set; }

    public int? Gender { get; set; }

    public string? WorkPosition { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public int Language { get; set; }

    public DateTime PasswordExpires { get; set; }

    public bool ChangePassword { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool Active { get; set; }

    public DateTime? Created { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<ThemeConfig> ThemeConfigs { get; set; } = new List<ThemeConfig>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
