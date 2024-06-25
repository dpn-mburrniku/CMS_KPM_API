namespace Entities.DataTransferObjects.WebDTOs
{
    public class SlidesModel
    {
        public int SllideId { get; set; }
        public string? SllideTitulli { get; set; }
        public string? SllidePershkrimi { get; set; }
        public DateTime? SllideDataInsertimit { get; set; }
        public string? Linku { get; set; }
        public int? OrderNr { get; set; }
        public int? GjuhaId { get; set; }
        public int? MediaId { get; set; }
        public MediaModel? media { get; set; }
    }
}
