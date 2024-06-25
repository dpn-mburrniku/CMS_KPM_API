using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{

    public partial class TagsDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public bool WebMultiLang { get; set; }
        public string Description { get; set; } = null!;
        public bool? Active { get; set; }

     }

    public partial class TagsListDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Description { get; set; } = null!;
        public bool? Active { get; set; }

    }

    public partial class AddTagsCollectionInPost
    {
        public List<int> PostTagId { get; set; }

        public bool WebMultiLang { get; set; }

        public int LanguageId { get; set; }

        public int PostId { get; set; }
       
    }

}
