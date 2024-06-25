using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PostMedium
{
    public int PostId { get; set; }

    public int LanguageId { get; set; }

    public int MediaId { get; set; }

    public bool IsSlider { get; set; }

    public int OrderNo { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Medium Media { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
