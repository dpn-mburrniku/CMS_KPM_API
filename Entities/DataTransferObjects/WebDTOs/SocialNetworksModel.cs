namespace Entities.DataTransferObjects.WebDTOs
{
    public class SocialNetworksModel
    {
        public int Id { get; set; }
        public int GjuhaId { get; set; }
        public int? LayoutId { get; set; }
        public string? Emertimi { get; set; }
        public string? Linku { get; set; }
        public string? ImgPath { get; set; }
        public string? Html { get; set; }
        public int? OrderNr { get; set; }
        public bool? Aktiv { get; set; }
    }
}
