using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class LiveChat
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public int? PageId { get; set; }

    public int? ParentId { get; set; }

    public int? Level { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsOtherSource { get; set; }

    public string? OtherSourceName { get; set; }

    public string? OtherSource { get; set; }

    public int? OrderNo { get; set; }

    public bool? Active { get; set; }

    public DateTime? Modified { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual ICollection<LiveChat> InverseLiveChatNavigation { get; set; } = new List<LiveChat>();

    public virtual Language Language { get; set; } = null!;

    public virtual LiveChat? LiveChatNavigation { get; set; }

    public virtual Page? Page { get; set; }
}
