using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Layout
{
    public int Id { get; set; }

    public string? NameSq { get; set; }

    public string? NameEn { get; set; }

    public string? NameSr { get; set; }

    public string? Description { get; set; }

    public string? Path { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Contact> Contacts { get; } = new List<Contact>();

    public virtual ICollection<Faqheader> Faqheaders { get; } = new List<Faqheader>();

    public virtual ICollection<GaleryHeader> GaleryHeaders { get; } = new List<GaleryHeader>();

    public virtual ICollection<LayoutRole> LayoutRoles { get; } = new List<LayoutRole>();

    public virtual ICollection<Link> Links { get; } = new List<Link>();

    public virtual ICollection<MenuType> MenuTypes { get; } = new List<MenuType>();

    public virtual ICollection<Page> Pages { get; } = new List<Page>();

    public virtual ICollection<Personel> Personels { get; } = new List<Personel>();

    public virtual ICollection<PostCategory> PostCategories { get; } = new List<PostCategory>();

    public virtual ICollection<Slide> Slides { get; } = new List<Slide>();

    public virtual ICollection<SocialNetwork> SocialNetworks { get; } = new List<SocialNetwork>();
}
