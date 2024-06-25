using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public partial class RolesDto
    {
        [JsonPropertyName("value")]
        public Guid Id { get; set; }

        [JsonPropertyName("label")]
        public string Name { get; set; }
        public string Name_EN { get; set; }        
        public string Name_SR { get; set; }
        public string Description { get; set; } 
    }

    public class RolesDtoAsync
    {
        [JsonPropertyName("value")]
        public Guid Id { get; set; }

        [JsonPropertyName("label")]
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NameSr { get; set; }
        public string Description { get; set; }
    }

    public class UserRole
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }

    public class AddRole
    {        
        [Required]
        public string Name_SQ { get; set; }
        [Required]
        public string Name_EN { get; set; }
        [Required]
        public string Name_SR { get; set; }
       
        public string? Description { get; set; }        
       
    }

    public class UpdateRole
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name_SQ { get; set; }
        [Required]
        public string Name_EN { get; set; }
        [Required]
        public string Name_SR { get; set; }
       
        public string? Description { get; set; }

    }

    public class AddMenuInRole
    {
        [Required] 
        public int SysMenuId { get;set; }
        [Required]
        public string RoleId { get; set; }
    }

    public class AddMenuCollectionInRole
    {
        [Required]
        public List<int> SysMenuId { get; set; }
        [Required]
        public string RoleId { get; set; }
    }



    public class MenusForRoleDto
    {
        public int Id { get; set; }       
        public string Name { get; set; }        
        public string Path { get; set; }
        public string Icon { get; set; }
        public int? Type { get; set; }
        public int? OrderNo { get; set; }   


    }

    public class RemoveMenuCollectionFromRole
    {
        [Required]
        public List<int> Ids { get; set; }        
    }

}
