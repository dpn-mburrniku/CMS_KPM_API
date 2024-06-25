using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MunicipalityLocation
{
    public int Id { get; set; }

    public int MunicipalityId { get; set; }

    public int? MediaId { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public decimal? Area { get; set; }

    public int? MeasureUnitId { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? MapLocationUrl { get; set; }

    public bool IsConfirmed { get; set; }

    public bool Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual MeasureUnit? MeasureUnit { get; set; }

    public virtual Medium? Media { get; set; }

    public virtual Municipality Municipality { get; set; } = null!;
}
