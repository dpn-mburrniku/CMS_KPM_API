using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Faqheader
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<Faqdetail> Faqdetails { get; } = new List<Faqdetail>();

    public virtual Layout Layout { get; set; } = null!;

    public virtual Page? Page { get; set; }
}
