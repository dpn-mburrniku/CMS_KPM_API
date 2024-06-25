using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class ResourceTranslationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ResourceTranslationString> ResourceTranslationStrings { get; set; } = new List<ResourceTranslationString>();
}
