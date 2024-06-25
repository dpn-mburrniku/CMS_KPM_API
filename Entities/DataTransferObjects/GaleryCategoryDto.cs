using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class GaleryCategoryDto
	{
		public int Id { get; set; }

		public string? NameSq { get; set; }

		public string? NameEn { get; set; }

		public string? NameSr { get; set; }

	}
	public class GaleryCategoryListDto
	{
		public int Id { get; set; }

		public string? NameSq { get; set; }

		public string? NameEn { get; set; }

		public string? NameSr { get; set; }

	}
}
