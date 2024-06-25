using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class LinkType
{
    public int LinkTypeId { get; set; }

    public int LanguageId { get; set; }

    public int ComponentLocationId { get; set; }

    public string? LinkuTypeName { get; set; }

    public virtual ComponentLocation ComponentLocation { get; set; } = null!;

    public virtual ICollection<Link> Links { get; set; } = new List<Link>();
}
