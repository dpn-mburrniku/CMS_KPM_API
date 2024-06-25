namespace Entities.DataTransferObjects.WebDTOs
{
    public class TemplateModel
    {
        public int TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public string? TemplateUrl { get; set; }
        public bool? TemplateUrlWithId { get; set; }
        public bool? NewsWithCategoryId { get; set; }
    }
}
