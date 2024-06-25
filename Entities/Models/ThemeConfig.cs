using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class ThemeConfig
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? ThemeColor { get; set; }

    public string? Mode { get; set; }

    public int? PrimaryColorLevel { get; set; }

    public string? NavMode { get; set; }

    public string? LayoutType { get; set; }

    public virtual AspNetUser? User { get; set; }
}
