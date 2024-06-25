using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class GaleryDetail
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int HeaderId { get; set; }

    public int MediaId { get; set; }

    public int OrderNo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual GaleryHeader GaleryHeader { get; set; } = null!;

    public virtual Medium Media { get; set; } = null!;
}
