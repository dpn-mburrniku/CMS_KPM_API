using Entities.Models; using CMS.API;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SlideDto
    {
        public int? Id { get; set; }
        public int LanguageId { get; set; }
        [Required]
        public int LayoutId { get; set; }
        public int? PageId { get; set; }        
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }        
        public int? OrderNo { get; set; }     
        public bool WebMultiLang { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class UpdateSlideDto
    {
        public int? Id { get; set; }
        public int LanguageId { get; set; }
        [Required]
        public int LayoutId { get; set; }
        public int? PageId { get; set; }
        public int? MediaId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? OrderNo { get; set; }
        public bool WebMultiLang { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class getSlideDto
    {
        public int? Id { get; set; }
        public int LanguageId { get; set; }
        [Required]
        public int LayoutId { get; set; }
        public int? PageId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int? OrderNo { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool WebMultiLang { get; set; }          
        public IFormFile? Image { get; set; }
        public virtual MediaListDto? Media { get; set; }
    }

    public class SlideListDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int LayoutId { get; set; }
        public int? PageId { get; set; }
        public int? MediaId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; } = null!;
        public virtual LayoutDto Layout { get; set; } = null!;
        public virtual MediaListDto? Media { get; set; }
        public virtual PageListDto? Page { get; set; }
    }

    public class SlideOtherSourceDto
    {
        public int? Id { get; set; }
        public int LanguageId { get; set; }
        [Required]
        public int LayoutId { get; set; }
        public int? PageId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int? OrderNo { get; set; }
        public bool WebMultiLang { get; set; }
        public string? OtherSourceLink { get; set; }
        public int MediaExCategoryId { get; set; }
    }

    public class UpdateSlideOrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }        
        [Required]
        public int OrderNo { get; set; }

    }
}
