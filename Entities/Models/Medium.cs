using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Medium
{
    public int Id { get; set; }

    public int MediaExCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public Guid FileName { get; set; }

    public string? FileNameMedium { get; set; }

    public string? FileNameSmall { get; set; }

    public string? FileEx { get; set; }

    public bool IsOtherSource { get; set; }

    public string? OtherSourceLink { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual MediaEx? FileExNavigation { get; set; }

    public virtual ICollection<GaleryDetail> GaleryDetails { get; set; } = new List<GaleryDetail>();

    public virtual MediaExCategory MediaExCategory { get; set; } = null!;

    public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();

    public virtual ICollection<MunicipalityLocation> MunicipalityLocations { get; set; } = new List<MunicipalityLocation>();

    public virtual ICollection<PageMedium> PageMedia { get; set; } = new List<PageMedium>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<Personel> Personels { get; set; } = new List<Personel>();

    public virtual ICollection<PostMedium> PostMedia { get; set; } = new List<PostMedium>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Slide> Slides { get; set; } = new List<Slide>();
}
