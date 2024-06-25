using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PostTag
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<PostsInTag> PostsInTags { get; set; } = new List<PostsInTag>();
}
