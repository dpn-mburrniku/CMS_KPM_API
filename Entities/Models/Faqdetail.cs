using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Faqdetail
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int HeaderId { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public int? OrderNo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Faqheader Faqheader { get; set; } = null!;
}
