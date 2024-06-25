using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class ComponentLocation
{
    public int Id { get; set; }

    public string TitleSq { get; set; } = null!;

    public string TitleEn { get; set; } = null!;

    public string TitleSr { get; set; } = null!;

    public int? OrderNo { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<LinkType> LinkTypes { get; } = new List<LinkType>();

    public virtual ICollection<SocialNetwork> SocialNetworks { get; } = new List<SocialNetwork>();
}
