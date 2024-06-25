using System;
using Entities.Models;
using System.Collections.Generic;

namespace CMS.API;

public partial class MenuLocation
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string? Title { get; set; }

    public int? MenuGroupId { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual MenuGroup? MenuGroup { get; set; }

    public virtual ICollection<Menu> Menus { get; } = new List<Menu>();
}
