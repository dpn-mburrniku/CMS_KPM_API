using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class SettingsDto
    {
        public string? SiteName { get; set; }
        public string? SiteDescription { get; set; }
        public string? SiteUrl { get; set; }
        public string? IISSiteName { get; set; }
        public string? LocalPath { get; set; }
        public string? PhotoLargeWidth { get; set; }
        public string? PhotoLargeHeight { get; set; }
        public string? PhotoMediumWidth { get; set; }
        public string? PhotoMediumHeight { get; set; }
        public string? PhotoSmallWidth { get; set; }
        public string? PhotoSmallHeight { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CMS_Version { get; set; }
        public string? MultiLanguage { get; set; }
        public string? ImageCrop { get; set; }
        public string? ImageResize { get; set; }
        public string? MaxFotoSize { get; set; }
        public string? MaxVideoSize { get; set; }
        public string? MaxFileSize { get; set; } 
        public string? DocumentUrlPath { get; set; }
    }

    public class UpdateExtraSettingsDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

    }
    public class ExtraSettingsDto
    {
        public int Id { get; set; }

        public string Label { get; set; }

        [Required]
        public string Value { get; set; }

    }
}
