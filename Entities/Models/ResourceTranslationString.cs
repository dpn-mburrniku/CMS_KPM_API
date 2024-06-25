using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class ResourceTranslationString
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int TypeId { get; set; }

    public string? Name { get; set; }

    public string? Value { get; set; }

    public virtual Language Language { get; set; } = null!;

    public virtual ResourceTranslationType Type { get; set; } = null!;
}
