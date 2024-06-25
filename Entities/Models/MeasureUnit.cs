using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MeasureUnit
{
    public int Id { get; set; }

    public string? Unit { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public virtual ICollection<MunicipalityLocation> MunicipalityLocations { get; } = new List<MunicipalityLocation>();
}
