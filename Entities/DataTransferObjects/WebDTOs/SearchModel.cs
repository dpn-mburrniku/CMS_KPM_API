namespace Entities.DataTransferObjects.WebDTOs
{
    public class SearchModel
    {        
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool OtherSource { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }
    }

    public class SearchCategory
    {
        public int CategoryId{ get; set; }
        public string CategoryName { get; set; }
        public int? CategoryCount { get; set; }
    }
}
