using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Slide
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public int? MediaId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Link { get; set; }

    public int? OrderNo { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;

    public virtual Medium? Media { get; set; }

    public virtual Page? Page { get; set; }
}
