using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TreeTypesForLocation
{
    public int Id { get; set; }

    public int MunicipalityLocationId { get; set; }

    public int TreeTypeId { get; set; }

    public int? TotalForPlanting { get; set; }

    public int? TotalPlanted { get; set; }

    public decimal? Price { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual MunicipalityLocation MunicipalityLocation { get; set; } = null!;

    public virtual TreeType TreeType { get; set; } = null!;
}
