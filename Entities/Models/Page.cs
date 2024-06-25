using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Page
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int LayoutId { get; set; }

    public int TemplateId { get; set; }

    public int? PageParentId { get; set; }

    public int? MediaId { get; set; }

    public string? PageName { get; set; }

    public string? PageText { get; set; }

    public string? PageAdress { get; set; }

    public bool Deleted { get; set; }

    public bool IsSubPage { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Faqheader> Faqheaders { get; set; } = new List<Faqheader>();

    public virtual ICollection<Page> InversePageNavigation { get; set; } = new List<Page>();

    public virtual Language Language { get; set; } = null!;

    public virtual Layout Layout { get; set; } = null!;

    public virtual ICollection<Link> Links { get; set; } = new List<Link>();

    public virtual ICollection<LiveChat> LiveChats { get; set; } = new List<LiveChat>();

    public virtual Medium? Media { get; set; }

    public virtual ICollection<Menu> MenuPage1s { get; set; } = new List<Menu>();

    public virtual ICollection<Menu> MenuPageNavigations { get; set; } = new List<Menu>();

    public virtual ICollection<Menu> MenuPages { get; set; } = new List<Menu>();

    public virtual ICollection<PageMedium> PageMedia { get; set; } = new List<PageMedium>();

    public virtual Page? PageNavigation { get; set; }

    public virtual ICollection<Personel> Personels { get; set; } = new List<Personel>();

    public virtual ICollection<PostCategory> PostCategories { get; set; } = new List<PostCategory>();

    public virtual ICollection<Slide> Slides { get; set; } = new List<Slide>();

    public virtual Template Template { get; set; } = null!;
}
