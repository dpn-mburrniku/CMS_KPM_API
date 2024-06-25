using Entities.Models;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class PageModel
    {
        public int PageId { get; set; }
        public int GjuhaId { get; set; }
        public int? MediaId { get; set; }
        public bool? hasMedia { get; set; }
        public bool? hasSlider { get; set; }
        public int? LayoutId { get; set; }
        public int? TemplateId { get; set; }
        public string? PageName { get; set; }
        public string? PageText { get; set; }
        public string? PageAdresa { get; set; }
        public string? PageLokacioni { get; set; }
        public bool? Fshire { get; set; }
        public bool? HasSubPages { get; set; }
        public virtual Medium? Media { get; set; }
        public virtual Layout? Layout { get; set; }
        public virtual Template? Template { get; set; }
    }
    public class PageWithDocsModel
    {
        public int PageId { get; set; }
        public int GjuhaId { get; set; }
        public int? MediaId { get; set; }
        public int? TemplateId { get; set; }
        public string? PageName { get; set; }
        public string? PageText { get; set; }
        public string? PageAdresa { get; set; }
        public string? PageLokacioni { get; set; }
        public bool? Fshire { get; set; }

        public virtual MediaModel? Media { get; set; }
        public virtual PageMedium? PageMedia { get; set; }
    }

    public class SubPagesListModel
    {
        public int Id { get; set; }
        public PageModel page { get; set; }
        public object? media { get; set; }
        public object? slider { get; set; }
        public List<VitetModel> vitet { get; set; }
        public int totalmediaRowsCount { get; set; }
        public int totalmediaPages { get; set; }
    }
}