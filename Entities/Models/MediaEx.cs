using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MediaEx
{
    public string MediaEx1 { get; set; } = null!;

    public int MediaExCategoryId { get; set; }

    public string? MediaExPath { get; set; }

    public virtual ICollection<Medium> Media { get; } = new List<Medium>();

    public virtual MediaExCategory MediaExCategory { get; set; } = null!;
}
