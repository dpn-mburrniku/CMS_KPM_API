using Entities.Models;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class LinksModel
    {
        public int LinkuId { get; set; }
        public string? Linku { get; set; }
        public string? LinkuHape { get; set; }
        public bool? LinkuAktiv { get; set; }
        public int? GjuhaId { get; set; }
        public string? LinkuPershkrimi { get; set; }
        public int? LinkuLlojiId { get; set; }
        public int? NrOrder { get; set; }
        public string LinkuLlojiPershkrimi { get; set; }
    }
}
