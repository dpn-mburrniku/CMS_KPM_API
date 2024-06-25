using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public class AddCollectionMenu
    {
        [Required]
        public List<int> PageId { get; set; }       
        [Required]
        public int MenuTypeId { get; set; }
    }

    public class MenuDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int MenuTypeId { get; set; }

        public int? MenuParentId { get; set; }

        public int? PageId { get; set; }

        public int? PageParentId { get; set; }

        public int? OrderNo { get; set; }

        public bool IsRedirect { get; set; }

        public int? PageIdredirect { get; set; }

        public bool IsOtherSource { get; set; }

        public string? OtherSourceName { get; set; }

        public string? OtherSourceUrl { get; set; }

        public string? Target { get; set; }

        public bool? Active { get; set; }

        public bool IsMegaMenu { get; set; }

        public bool? IsClickable { get; set; }       
    }

    public class UpdateMenuDto 
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int? PageId { get; set; }

        public int? PageParentId { get; set; }

        public int? OrderNo { get; set; }

        public bool IsRedirect { get; set; }

        public bool IsOtherSource { get; set; }

        public string? OtherSourceName { get; set; }

        public int? PageIdredirect { get; set; }

        public string? OtherSourceUrl { get; set; }

        public string? Target { get; set; }

        public bool? Active { get; set; }

        public bool IsMegaMenu { get; set; }

        public bool? IsClickable { get; set; }
    }

    public class MenuListDto
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public int? PageId { get; set; }

        public int? PageParentId { get; set; }

        public int? OrderNo { get; set; }

        public bool IsRedirect { get; set; }

        public int? PageIdredirect { get; set; }

        public bool IsOtherSource { get; set; }

        public string? OtherSourceName { get; set; }

        public string? OtherSourceUrl { get; set; }

        public string? Target { get; set; }

        public bool? Active { get; set; }

        public bool IsMegaMenu { get; set; }

        public bool? IsClickable { get; set; }

        public virtual PageJoinDto? Page { get; set; }

        public virtual PageJoinDto? PageParent { get; set; }

        public virtual List<MenuListDto>? Children { get; set; }
    }

    public class UpdatMenuOrderDto
    {
        [Required]
        public int Id { get; set; }        

        public int? MenuParentId { get; set; } = 0;
        [Required]
        public int OrderNo { get; set; }
        public int Level { get; set; }

    }

    public class MenuTypeDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int? LayoutId { get; set; }

        public string? Title { get; set; }

        public bool WebMultiLang { get; set; }          
    }

    public class UpdateMenuTypeDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int? LayoutId { get; set; }

        public string? Title { get; set; }
        
    }

    public class MenuOtherSourceDto
    {
        public int Id { get; set; }
        [Required]
        public int MenuTypeId { get; set; }
        [Required]
        public string OtherSourceName { get; set; }
        [Required]
        public string OtherSourceUrl { get; set; }
        [Required]
        public string Target { get; set; }  
    }
}

