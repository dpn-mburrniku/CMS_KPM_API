using Entities.Models; using CMS.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.DataTransferObjects
{
    public class PostDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public bool WebMultiLang { get; set; }
        public int? MediaId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public bool Published { get; set; }
        public string? StartDateStr { get; set; }
        public string? EndDateStr { get; set; }
        public string? EventDateStr { get; set; }
        public List<int> PostCategoryIds { get; set;}

        public List<int>? PostTagsIds { get; set; }
    }

    public partial class PostUpdate
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int? MediaId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }    
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int ClicksNo { get; set; }
        public string? StartDateStr { get; set; }
        public string? EndDateStr { get; set; }
        public string? EventDateStr { get; set; }
        public List<int> PostCategoryIds { get; set; }
        public List<int>? PostTagsIds { get; set; }
    }

    public class PostListDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int? MediaId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EventDate { get; set; }
        public bool? Published { get; set; }
        public bool? Deleted { get; set; }
        public int? ClicksNo { get; set; }
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; } = null!;
        public virtual MediaListDto? Media { get; set; }
        public virtual ICollection<PostsInCategoryDto> PostsInCategories { get; } = new List<PostsInCategoryDto>();
        public virtual ICollection<PostsInTagDto>? PostsInTags { get; } = new List<PostsInTagDto>();
    }

    public class AddMediaCollectionInPost
    {
        [Required]
        public List<int> MediaId { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public bool WebMultiLang { get; set; }
        [Required]
        public int webLangId { get; set; }
        [Required]
        public bool IsSlider { get; set; }

    }

    public class GetPostMediaDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }

        public int LanguageId { get; set; }

        public int MediaId { get; set; }

        public int OrderNo { get; set; }

        public bool IsSlider { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; } = null!;

        public virtual Medium Media { get; set; } = null!;
        
    }

    public class UpdatePostMediaOrderDto
    {   
        [Required]
        public int PostId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int MediaId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }
}

