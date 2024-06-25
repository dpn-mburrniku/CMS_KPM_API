using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PageMedium
{
    public int PageId { get; set; }

    public int LanguageId { get; set; }

    public int MediaId { get; set; }

    public string? Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int OrderNo { get; set; }

    public bool IsSlider { get; set; }

    public int? DocumentParentId { get; set; }

    public string? Link { get; set; }

    public string? LinkName { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Medium Media { get; set; } = null!;

    public virtual Page Page { get; set; } = null!;
}
