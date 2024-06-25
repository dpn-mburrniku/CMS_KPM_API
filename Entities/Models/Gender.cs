using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Gender
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
