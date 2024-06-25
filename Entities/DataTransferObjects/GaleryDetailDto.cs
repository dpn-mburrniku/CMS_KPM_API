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
	public class GaleryDetailDto
	{
		public int Id { get; set; }

        public int webLangId { get; set; }

        public bool WebMultiLang { get; set; }

		public int HeaderId { get; set; }

		public List<int> MediaIds { get; set; }

	}

	public class GaleryDetailListDto
	{
		public int Id { get; set; }

		public int LanguageId { get; set; }

		public int HeaderId { get; set; }

		public int MediaId { get; set; }

		public int? OrderNo { get; set; }		

		public virtual GaleryHeaderDto GaleryHeader { get; set; } = null!;

		public virtual Medium Media { get; set; }
	}

    public class UpdateGaleryMediaOrderDto
    {
        [Required]
        public int HeaderId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int MediaId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }
}
