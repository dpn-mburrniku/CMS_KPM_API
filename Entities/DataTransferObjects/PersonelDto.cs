using Entities.Models;
using CMS.API;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
	public class PersonelDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public int? PageId { get; set; }

		//public int? MediaId { get; set; }

		public string? Name { get; set; }

		public string? LastName { get; set; }

		public string? Position { get; set; }

		public string? Qualification { get; set; }

		public string? BirthDateStr { get; set; }

		public string? BirthPlace { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Email { get; set; }

		public string? OtherInfo { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }
		public IFormFile? Image { get; set; }
	}

	public class PersonelListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public int? PageId { get; set; }

		public int? MediaId { get; set; }

		public string? Name { get; set; }

		public string? LastName { get; set; }

		public string? Position { get; set; }

		public string? Qualification { get; set; }

		public string? BirthDateStr { get; set; }

		public string? BirthPlace { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Email { get; set; }

		public string? OtherInfo { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }

		public virtual LayoutDto Layout { get; set; } = null!;

		public virtual PageDto? Page { get; set; }
		public virtual MediaListDto? Media { get; set; }
	}

	public class PersonelGetByIdDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public int LayoutId { get; set; }

		public int? PageId { get; set; }

		public int? MediaId { get; set; }

		public string? Name { get; set; }

		public string? LastName { get; set; }

		public string? Position { get; set; }

		public string? Qualification { get; set; }

		public string? BirthDateStr { get; set; }

		public string? BirthPlace { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Email { get; set; }

		public string? OtherInfo { get; set; }

		public int? OrderNo { get; set; }

		public bool? Active { get; set; }

		public DateTime? Modified { get; set; }

		public string? ModifiedBy { get; set; }

		public DateTime Created { get; set; }

		public string CreatedBy { get; set; } = null!;

		public virtual MediaListDto? Media { get; set; }
	}

	public class UpdateOrderPersonelDto
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int LanguageId { get; set; }
		[Required]
		public int OrderNo { get; set; }
	}
}
