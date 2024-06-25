using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class UserAudit
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public byte ActionType { get; set; }

    public string DescriptionSq { get; set; } = null!;

    public string DescriptionEn { get; set; } = null!;

    public string DescriptionSr { get; set; } = null!;

    public DateTime Date { get; set; }
}
