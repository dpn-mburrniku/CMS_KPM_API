using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class PostCategory
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int? PageId { get; set; }

    public string Title { get; set; } = null!;

    public bool? Active { get; set; }

    public string? Extra { get; set; }

    public bool? ShowInFilters { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;

    public virtual Page? Page { get; set; }

    public virtual ICollection<PostsInCategory> PostsInCategories { get; set; } = new List<PostsInCategory>();
}
