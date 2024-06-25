using Entities.Models;

namespace Entities.DataTransferObjects.WebDTOs
{
	public class PersonelModel
	{
		public int PersoneliId { get; set; }
		public int GjuhaId { get; set; }
		public string? PersoneliEmri { get; set; }
		public string? PersoneliMbiemri { get; set; }
		public string? PersoneliPozita { get; set; }
		public string? PersoneliKualifikimi { get; set; }
		public DateTime? PersoneliDataLindjes { get; set; }
		public string? PresoneliVendiLindjes { get; set; }
		public string? PersoneliNrTelefonit { get; set; }
		public string? PersoneliEmail { get; set; }
		public string? PersoneliInformataShtes { get; set; }
		public int? PersoneliOrderNr { get; set; }
		public bool? PersoneliAktiv { get; set; }
		public int? PageId { get; set; }
		public string? PageName { get; set; }
		public string? Url { get; set; }
		public int? MediaId { get; set; }
		public MediaModel? media { get; set; }
	}
}
