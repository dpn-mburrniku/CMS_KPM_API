using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class EmailTemplate
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<EmailTemplateItem> EmailTemplateItems { get; set; } = new List<EmailTemplateItem>();
}
