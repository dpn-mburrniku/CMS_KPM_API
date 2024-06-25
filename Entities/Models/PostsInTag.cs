using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PostsInTag
{
    public int PostTagId { get; set; }

    public int LanguageId { get; set; }

    public int PostId { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;

    public virtual PostTag PostTag { get; set; } = null!;
}
