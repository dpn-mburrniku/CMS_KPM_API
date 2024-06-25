using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TreeType
{
    public int Id { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public decimal? Price { get; set; }

    public bool Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<CertificateTypeConfigurtation> CertificateTypeConfigurtations { get; } = new List<CertificateTypeConfigurtation>();

    public virtual ICollection<Certificate> Certificates { get; } = new List<Certificate>();

    public virtual ICollection<TreeTypesForLocation> TreeTypesForLocations { get; } = new List<TreeTypesForLocation>();
}
