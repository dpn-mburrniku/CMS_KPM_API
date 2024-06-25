using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Municipality
{
    public int Id { get; set; }

    public int? MediaId { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public bool Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Medium? Media { get; set; }

    public virtual ICollection<MunicipalityLocation> MunicipalityLocations { get; set; } = new List<MunicipalityLocation>();
}
