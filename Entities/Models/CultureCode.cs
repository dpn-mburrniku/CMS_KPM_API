using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class CultureCode
{
    public int Id { get; set; }

    public string? CultureCode1 { get; set; }

    public string? CultureCodeCountry { get; set; }

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
}
