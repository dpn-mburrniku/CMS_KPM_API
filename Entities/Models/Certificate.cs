using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Certificate
{
    public int Id { get; set; }

    public string? Reference { get; set; }

    public int? MediaId { get; set; }

    public int? MunicipalityId { get; set; }

    public int? LocationId { get; set; }

    public int? TreeTypeId { get; set; }

    public int? TypeId { get; set; }

    public int? Amount { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? FromText { get; set; }

    public string? PersonalisedText { get; set; }

    public string? Color { get; set; }

    public string? QrcodeLink { get; set; }

    public int? PaymentId { get; set; }

    public bool IsDownloaded { get; set; }

    public bool Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public virtual MunicipalityLocation? Location { get; set; }

    public virtual Medium? Media { get; set; }

    public virtual Municipality? Municipality { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual TreeType? TreeType { get; set; }

    public virtual CertificateType? Type { get; set; }
}
