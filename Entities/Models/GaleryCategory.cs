using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class GaleryCategory
{
    public int Id { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public virtual ICollection<GaleryHeader> GaleryHeaders { get; set; } = new List<GaleryHeader>();
}
