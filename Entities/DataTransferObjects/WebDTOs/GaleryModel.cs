using Entities.Models;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class GaleryModel
    {
        public int MediaGaleriaId { get; set; }
        public string? MediaGaleriaPershkrimi { get; set; }
        public int? MediaGaleriaKategoriaId { get; set; }
        public int? OrderNr { get; set; }
        public bool? Fshire { get; set; }
        public string Url { get; set; }
        public MediaModel? media { get; set; }
    }
    public class GaleryDetailsModel
    {
        public int MediaGaleriaId { get; set; }
        public string? MediaGaleriaPershkrimi { get; set; }
        public int? MediaGaleriaKategoriaId { get; set; }
        public int MediaId { get; set; }
        public int? OrderNr { get; set; }
        public bool? Fshire { get; set; }
        public List<MediaModel>? media { get; set; }
    }
}
