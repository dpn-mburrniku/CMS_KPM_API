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
    public class MediaDto
    {
        //public int? MediaExCategoryId { get; set; }

        //public string? Name { get; set; } 

        //public Guid? FileName { get; set; }

        //public string? FileNameMedium { get; set; }

        //public string? FileNameSmall { get; set; }

        //public string? FileEx { get; set; } 

        //public bool? IsOtherSource { get; set; }

        //public string? OtherSourceLink { get; set; }
        [Required]
        public List<IFormFile> Files { get; set; }
        public bool? IsCrop { get; set; } = false;
        public int? CropWidth { get; set; }
        public int? CropHeight { get; set; }
        public bool? Resize { get; set; } = false;
    }

    public class MediaListDto
    {
        public int Id { get; set; }

        public int MediaExCategoryId { get; set; }
        public string Name { get; set; } = null!;

        public Guid FileName { get; set; }

        public string? FileNameMedium { get; set; }

        public string? FileNameSmall { get; set; }

        public string FileEx { get; set; } = null!;

        public bool IsOtherSource { get; set; }

        public string? OtherSourceLink { get; set; }

        public DateTime? Modified { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; } = null!;         

        public virtual MediaExCategoryDto MediaExCategory { get; set; } = null!;
        public virtual MediaExDto? FileExNavigation { get; set; }
    }

    public class MediaExCategoryDto
    {
        public int Id { get; set; }

        public string? NameSq { get; set; }

        public string? NameEn { get; set; }

        public string? NameSr { get; set; }
    }

    public class MediaOtherSourceDto
    {
        [Required]
        public int MediaExCategoryId { get; set; }
        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? OtherSourceLink { get; set; }
       
    }

    public partial class MediaExDto
    {
        public string MediaEx1 { get; set; } = null!;

        public int MediaExCategoryId { get; set; }

        public string? MediaExPath { get; set; }
        
    }

    public class Data
    {
        public int Code { get; set; } = 220;
        public List<Source> sources { get; set; }   
    }

    public class Source
    {
        public string baseurl { get; set; } = "http://samirshehu:5005/Media/";      //"Media/";
        public List<Files> files { get; set; }
        public string name { get; set; } = "default";
        public string path { get; set; } = "";
    }

    public class Files
    {
        public string changed { get; set; }
        public string file { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool isImage { get; set; }
        public string size { get; set; }
        public string thumb { get; set; }
    }
}
