using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class FAQModel
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int LayoutId { get; set; }

        public int? PageId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<FAQDetailsModel> Faqdetails { get; set; } = new List<FAQDetailsModel>();
    }
    public class FAQDetailsModel
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int HeaderId { get; set; }

        public string? Question { get; set; }

        public string? Answer { get; set; }

        public int? OrderNo { get; set; }
    }
}
