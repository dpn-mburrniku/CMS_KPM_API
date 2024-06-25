using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Contact
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public int? MediaId { get; set; }

    public int? GenderId { get; set; }

    public string? Description { get; set; }

    public string? ContactPerson { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PhoneNumber2 { get; set; }

    public string? Fax { get; set; }

    public string? Email { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public string? MapLocation { get; set; }

    public int OrderNo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Gender? Gender { get; set; }

    public virtual Layout Layout { get; set; } = null!;

    public virtual Medium? Media { get; set; }

    public virtual Page? Page { get; set; }
}
