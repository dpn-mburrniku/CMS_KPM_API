using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class GaleryHeader
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public int OrderNo { get; set; }

    public bool IsDeleted { get; set; }

    public bool? ShfaqNeHome { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual GaleryCategory Category { get; set; } = null!;

    public virtual ICollection<GaleryDetail> GaleryDetails { get; } = new List<GaleryDetail>();

    public virtual Layout Layout { get; set; } = null!;
}
