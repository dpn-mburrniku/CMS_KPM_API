using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class CertificateTypeConfigurtation
{
    public int Id { get; set; }

    public int CertificateTypeId { get; set; }

    public int TreeTypeId { get; set; }

    public int? AmountFrom { get; set; }

    public int? AmountTo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual CertificateType CertificateType { get; set; } = null!;

    public virtual TreeType TreeType { get; set; } = null!;
}
