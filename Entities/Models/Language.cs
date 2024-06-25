using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Language
{
    public int Id { get; set; }

    public int? CultureCodeId { get; set; }

    public string NameSq { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public bool? Active { get; set; }

    public virtual CultureCode? CultureCode { get; set; }

    public virtual ICollection<LiveChat> LiveChats { get; set; } = new List<LiveChat>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<ResourceTranslationString> ResourceTranslationStrings { get; set; } = new List<ResourceTranslationString>();
}
