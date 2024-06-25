using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MediaExCategory
{
    public int Id { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public virtual ICollection<Medium> Media { get; } = new List<Medium>();

    public virtual ICollection<MediaEx> MediaExes { get; } = new List<MediaEx>();
}
