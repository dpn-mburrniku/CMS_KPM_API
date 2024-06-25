using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Link
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public int TypeId { get; set; }

    public string LinkName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string LinkTarget { get; set; } = null!;

    public bool? Active { get; set; }

    public int OrderNo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;

    public virtual LinkType LinkType { get; set; } = null!;

    public virtual Page? Page { get; set; }
}
