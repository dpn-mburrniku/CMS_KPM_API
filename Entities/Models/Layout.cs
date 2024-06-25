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

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Faqheader> Faqheaders { get; set; } = new List<Faqheader>();

    public virtual ICollection<GaleryHeader> GaleryHeaders { get; set; } = new List<GaleryHeader>();

    public virtual ICollection<LayoutRole> LayoutRoles { get; set; } = new List<LayoutRole>();

    public virtual ICollection<Link> Links { get; set; } = new List<Link>();

    public virtual ICollection<MenuType> MenuTypes { get; set; } = new List<MenuType>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<Personel> Personels { get; set; } = new List<Personel>();

    public virtual ICollection<PostCategory> PostCategories { get; set; } = new List<PostCategory>();

    public virtual ICollection<Slide> Slides { get; set; } = new List<Slide>();

    public virtual ICollection<SocialNetwork> SocialNetworks { get; set; } = new List<SocialNetwork>();
}
