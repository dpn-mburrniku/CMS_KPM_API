namespace Entities.DataTransferObjects.WebDTOs
{
    public class LiveChatModel
    {
        public int Id { get; set; }
        public int GjuhaId { get; set; }
        public int? Level { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? PageId { get; set; }
        public bool? IsOtherSource { get; set; }
        public string? LinkName { get; set; }
        public string? Url { get; set; }
        public string? handler { get; set; }
        public bool? HasChilds { get; set; }
        public List<ChildLiveChatModel>? childs { get; set; }
        public int? OrderNr { get; set; }
    }
    public class ChildLiveChatModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? OrderNr { get; set; }
        public string handler { get; set; }
    }
}
