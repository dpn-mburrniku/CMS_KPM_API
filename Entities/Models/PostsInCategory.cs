using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PostsInCategory
{
    public int PostCategoryId { get; set; }

    public int PostId { get; set; }

    public int LanguageId { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;

    public virtual PostCategory PostCategory { get; set; } = null!;
}
