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
    public class PageDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int? LayoutId { get; set; }

        public int TemplateId { get; set; }

        public int? PageParentId { get; set; }

        public int? MediaId { get; set; }
        [Required]
        public string? PageName { get; set; }

        public string? PageText { get; set; }

        public string? PageAdress { get; set; }

        public bool Deleted { get; set; } = false;

        public bool IsSubPage { get; set; } = false;
       
        public bool WebMultiLang { get; set; }
    }
    public class PageJoinDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }
        public string? PageName { get; set; }
        public string? PageNameWithId
        {
            get
            {
                string result = "ID:" + Id + " - " + PageName;
                return result;
            }
        }
    }

    public class PageListDto
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public int LayoutId { get; set; }

        public int TemplateId { get; set; }

        public int? PageParentId { get; set; }

        public int? MediaId { get; set; }

        public string? PageName { get; set; }        

        public string? PageNameWithId
        {
            get
            {
                string result = "ID:" + Id + " - " + PageName;
                return result;
            }
        }

        public string? PageAdress { get; set; }

        public bool Deleted { get; set; }

        public bool IsSubPage { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; } = null!;

        public virtual LayoutDto Layout { get; set; } = null!;

        public virtual Medium? Media { get; set; }
        public virtual TemplateDto Template { get; set; } = null!;

        public virtual ICollection<Page> SubPages { get; } = new List<Page>();
        public int CountSubPages
        {
            get
            {
                return SubPages.Where(x=>x.Deleted != true).Count();
            }
        }
    }

    public class AddMediaCollectionInPage
    {
        [Required]
        public List<int> MediaId { get; set; }
        [Required]
        public int PageId { get; set; }
        [Required]
        public bool IsSlider { get; set; }
        [Required]
        public bool WebMultiLang { get; set; }
        [Required]
        public int webLangId { get; set; }
    }

    public class GetPageMediaDto
    {
        public int Id { get; set; }
        public int PageId { get; set; }

        public int LanguageId { get; set; }

        public int MediaId { get; set; }

        public string? Name { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public int? OrderNo { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; } = null!;

        public virtual Medium Media { get; set; } = null!;

    }

    public class UpdatePageMediaOrderDto
    {
        [Required]
        public int PageId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int MediaId { get; set; }
        [Required]
        public int OrderNo { get; set; }

    }


    public partial class UpdateMediaInPage
    {
        public int PageId { get; set; }

        public int LanguageId { get; set; }

        public int MediaId { get; set; }

        public string? Name { get; set; }

        public string StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? Link { get; set; }

        public string? LinkName { get; set; }

    }

    public partial class CheckPageBeforeDeleteList
    {
        public int Menus { get; set; }
        public int Documents { get; set; }
        public int Media { get; set; }
        public int SubPages { get; set; }
        public string Gjuha { get; set; }
        public int GjuhaId { get; set; }

    }
}
