using Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class MunicipalityLocationDto
	{
		public int Id { get; set; }

		public int MunicipalityId { get; set; }

		public int? MediaId { get; set; }

		public string? NameSq { get; set; }

		public string? NameEn { get; set; }

		public string? NameSr { get; set; }

		public decimal? Area { get; set; }

		public int? MeasureUnitId { get; set; }

		public string? Latitude { get; set; }

		public string? Longitude { get; set; }

		public string? MapLocationUrl { get; set; }

		public bool IsConfirmed { get; set; }

		public bool? Active { get; set; }

		public IFormFile? Image { get; set; }
	}

	public class getMunicipalityLocationDto
	{
		public int Id { get; set; }

		public int MunicipalityId { get; set; }

		public int? MediaId { get; set; }

		public string? NameSq { get; set; }

		public string? NameEn { get; set; }

		public string? NameSr { get; set; }

		public decimal? Area { get; set; }

		public int? MeasureUnitId { get; set; }

		public string? Latitude { get; set; }

		public string? Longitude { get; set; }

		public string? MapLocationUrl { get; set; }

		public bool IsConfirmed { get; set; }

		public bool? Active { get; set; }

		public IFormFile? Image { get; set; }

		public virtual MediaListDto? Media { get; set; }
	}

	public class MunicipalityLocationListDto
	{
		public int Id { get; set; }

		public int MunicipalityId { get; set; }

		public int? MediaId { get; set; }

		public string? NameSq { get; set; }

		public string? NameEn { get; set; }

		public string? NameSr { get; set; }

		public decimal? Area { get; set; }

		public int? MeasureUnitId { get; set; }

		public string? Latitude { get; set; }

		public string? Longitude { get; set; }

		public string? MapLocationUrl { get; set; }

		public bool IsConfirmed { get; set; }

		public bool? Active { get; set; }

		public virtual MeasureUnitDto? MeasureUnit { get; set; }

		public virtual MunicipalityDto Municipality { get; set; } = null!;

		public virtual MediaListDto? Media { get; set; }
	}

    public class LocationCordinateListDto
    {
        public int Id { get; set; }

        public int MunicipalityId { get; set; }        

        public string? NameSq { get; set; }

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }

        public decimal? Area { get; set; }

        public int? MeasureUnitId { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public string? MapLocationUrl { get; set; }

        public bool IsConfirmed { get; set; }

        public bool? Active { get; set; }     
		
		public int TotalForPlantingForLocation { get; set; }
		public int TotalPlantedForLocation { get; set; }

        public virtual MeasureUnitDto? MeasureUnit { get; set; }

        public virtual MunicipalityDto Municipality { get; set; } = null!;
         
    }

}
