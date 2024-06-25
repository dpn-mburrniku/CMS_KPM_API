using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Post
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int? MediaId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Content { get; set; }

    public string? Location { get; set; }

    public string? Address { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? EventDate { get; set; }

    public bool Published { get; set; }

    public bool Deleted { get; set; }

    public int ClicksNo { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Medium? Media { get; set; }

    public virtual ICollection<PostMedium> PostMedia { get; } = new List<PostMedium>();

    public virtual ICollection<PostsInCategory> PostsInCategories { get; } = new List<PostsInCategory>();

    public virtual ICollection<PostsInTag> PostsInTags { get; } = new List<PostsInTag>();
}
