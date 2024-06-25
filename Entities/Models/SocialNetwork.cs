using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class SocialNetwork
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int ComponentLocationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Link { get; set; }

    public string? ImgPath { get; set; }

    public string? Html { get; set; }

    public int? OrderNo { get; set; }

    public bool? Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ComponentLocation ComponentLocation { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;
}
