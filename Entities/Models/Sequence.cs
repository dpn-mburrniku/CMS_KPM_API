using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Sequence
{
    public string Name { get; set; } = null!;

    public int Seed { get; set; }

    public int Incr { get; set; }

    public int? Currval { get; set; }
}
