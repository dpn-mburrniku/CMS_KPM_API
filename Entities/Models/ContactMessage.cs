using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class ContactMessage
{
    public int Id { get; set; }

    public int? LayoutId { get; set; }

    public int? PageId { get; set; }

    public int? ContactId { get; set; }

    public string? EmailFrom { get; set; }

    public string? EmailTo { get; set; }

    public string? EmailCc { get; set; }

    public string? EmailBcc { get; set; }

    public string? Name { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public bool IsDraft { get; set; }

    public bool IsFavorite { get; set; }

    public bool IsSent { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }
}
