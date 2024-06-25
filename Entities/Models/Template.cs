using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Template
{
    public int Id { get; set; }

    public string? TemplateName { get; set; }

    public string? TemplateUrl { get; set; }

    public bool? TemplateUrlWithId { get; set; }

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
}
