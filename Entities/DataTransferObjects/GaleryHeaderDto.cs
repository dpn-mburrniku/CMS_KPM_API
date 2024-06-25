using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
	public class GaleryHeaderDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public bool WebMultiLang { get; set; }

		public int LayoutId { get; set; }

		public int CategoryId { get; set; }

		public string Title { get; set; } = null!;

		public int OrderNo { get; set; }

		public bool IsDeleted { get; set; }

		public bool? ShfaqNeHome { get; set; }
		
	}
	public class GaleryHeaderListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public int LayoutId { get; set; }

		public int CategoryId { get; set; }

		public string Title { get; set; } = null!;

		public int OrderNo { get; set; }

		public bool IsDeleted { get; set; }

		public bool? ShfaqNeHome { get; set; }

		public virtual GaleryCategory Category { get; set; } = null!;

        public virtual ICollection<GaleryDetailDto> GaleryDetails { get; } = new List<GaleryDetailDto>();

		public int detailsNo { get
			{ return GaleryDetails.Count; } 
		}

        public virtual LayoutDto Layout { get; set; } = null!;

	}

    public class UpdateGaleryHeaderOrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }
}
