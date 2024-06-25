using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string Label { get; set; } = null!;

    public string? Value { get; set; }
}
