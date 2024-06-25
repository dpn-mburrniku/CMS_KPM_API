using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class EmailTemplateItem
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int EmailTemplateId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual EmailTemplate EmailTemplate { get; set; } = null!;
}
