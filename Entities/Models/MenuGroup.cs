using System;
using System.Collections.Generic;

namespace CMS.API;

public partial class MenuGroup
{
    public int Id { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public virtual ICollection<MenuLocation> MenuLocations { get; } = new List<MenuLocation>();
}
