using Entities.Models;
using System;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class NewsModel
    {
        public int? PostimiId { get; set; }
        public string? PostimiTitulli { get; set; }
        public string? PostimiPershkrimi { get; set; }
        public string? PostimiPermbajtja { get; set; }
        public string? PostimiAdresa { get; set; }
        public string? PostimiLokacioni { get; set; }
        public string Url { get; set; }
        public DateTime? PostimiDataInstertimit { get; set; }
        public DateTime? PostimiDataFillimit { get; set; }
        public DateTime? PostimiDataMbarimit { get; set; }
        public DateTime? PostimiDataNgjarjes { get; set; }
        public bool? Publikuar { get; set; }
        public int? GjuhaId { get; set; }
        public string? PostimiKategoria { get; set; }
        public MediaModel? media { get; set; }
        public MediaModel? docs { get; set; }
    }

    public class NewsCategoriesModel
    {
        public int PostimetKategoriaId { get; set; }
        public string PostimetKategoriaPershkrimi { get; set; }
        public int PageId { get; set; }

    }
}
