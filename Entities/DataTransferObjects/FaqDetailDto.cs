using Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class FaqDetailDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public bool WebMultiLang { get; set; }

        public int HeaderId { get; set; }

        public string? Question { get; set; }

        public string? Answer { get; set; }

    }
    public class FaqDetailListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int HeaderId { get; set; }

        public string? Question { get; set; }

        public string? Answer { get; set; }

        public virtual Faqheader Faqheader { get; set; } = null!;
    }

    public class UpdateFaqDetailListDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }        
        [Required]
        public int OrderNo { get; set; }
    }
    public class UpdateFaqDetailsOrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int HeaderId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
       
        public int OrderNo { get; set; }

    }
}
