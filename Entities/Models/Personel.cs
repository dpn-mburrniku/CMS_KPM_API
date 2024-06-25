using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Personel
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public int? MediaId { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public string? Qualification { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? BirthPlace { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? OtherInfo { get; set; }

    public int? OrderNo { get; set; }

    public bool? Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;

    public virtual Medium? Media { get; set; }

    public virtual Page? Page { get; set; }
}
