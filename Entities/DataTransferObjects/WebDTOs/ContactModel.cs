
using System.Numerics;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class ContactModel
    {
        public int KontaktiId { get; set; }
        public string? KontaktiPershkrimi { get; set; }
        public string? PersoniKontaktues { get; set; }
        public string? Adresa { get; set; }
        public string? Telefoni { get; set; }
        public string? Telefoni2 { get; set; }
        public string? Fax { get; set; }
        public string? Emaili { get; set; }
        public int? MediaId { get; set; }
        public int? PageId { get; set; }
        public string? PageName { get; set; }
        public int? GjuhaId { get; set; }
        public string? Gps { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public MediaModel? media { get; set; }
    }

    public class EmailDto
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public string Message { get; set;}
    }

    public class rechapchaResponde
    {
        public bool success { get; set; }
        public string hostname { get; set; }
    }    
}
