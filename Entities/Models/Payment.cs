using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Payment
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string? Email { get; set; }

    public decimal Amount { get; set; }

    public string? Request { get; set; }

    public bool Succeed { get; set; }

    public string? ResponseFromBank { get; set; }

    public virtual ICollection<Certificate> Certificates { get; } = new List<Certificate>();
}
