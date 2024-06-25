using Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class MediaModel
    {
        public int MediaId { get; set; }
        public int? MediaExKategoriaId { get; set; }
        public string? MediaEmri { get; set; }
        public string? MediaEmri_medium { get; set; }
        public string? MediaEmri_small { get; set; }
        public string? MediaEx { get; set; }
        public string? MediaPershkrimi { get; set; }
        public DateTime? MediaDataInsertimit { get; set; }
        public bool? IsOtherSource { get; set; }
        public string? OtherSource { get; set; }
        public bool IsSlider { get; set; }
        public int OrderNo { get; set; }
    }
}
