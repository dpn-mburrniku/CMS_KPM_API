using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class Menu
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int MenuTypeId { get; set; }

    public int? MenuParentId { get; set; }

    public int? PageId { get; set; }

    public int? PageParentId { get; set; }

    public int Level { get; set; }

    public int? OrderNo { get; set; }

    public bool IsRedirect { get; set; }

    public int? PageIdredirect { get; set; }

    public bool IsOtherSource { get; set; }

    public string? OtherSourceName { get; set; }

    public string? OtherSourceUrl { get; set; }

    public string? Target { get; set; }

    public bool? Active { get; set; }

    public bool IsMegaMenu { get; set; }

    public bool? IsClickable { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<Menu> InverseMenuNavigation { get; } = new List<Menu>();

    public virtual Menu? MenuNavigation { get; set; }

    public virtual MenuType MenuType { get; set; } = null!;

    public virtual Page? Page { get; set; }

    public virtual Page? Page1 { get; set; }

    public virtual Page? PageNavigation { get; set; }
}
