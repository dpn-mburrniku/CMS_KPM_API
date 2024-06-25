using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MenuType
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int? LayoutId { get; set; }

    public string? Title { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Layout? Layout { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
