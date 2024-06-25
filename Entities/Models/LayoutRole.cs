using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class LayoutRole
{
    public int Id { get; set; }

    public string RoleId { get; set; } = null!;

    public int LayoutId { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;
}
