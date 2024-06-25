using Entities.Models;
using System;
using System.Collections.Generic;

namespace CMS.API;

public partial class PostsInMedium
{
    public int Id { get; set; }

    public int MediaId { get; set; }

    public int PostId { get; set; }

    public int LanguageId { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Medium Media { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
