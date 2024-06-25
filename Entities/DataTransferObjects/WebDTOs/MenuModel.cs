using Entities.Models;

namespace Entities.DataTransferObjects.WebDTOs
{
    public class MenuModel
    {
        public int MenuId { get; set; }
        public string? MenuLocationName { get; set; }
        public int? MenuLocationId { get; set; }
        public int? MenuLocationGroupID { get; set; }
        public int? MenuPrindId { get; set; }
        public int? LayoutID { get; set; }
        public int? PageId { get; set; }
        public int? PagePrindId { get; set; }
        public int? NrOrder { get; set; }
        public bool? Redirect { get; set; }
        public int? PageIdredirect { get; set; }
        public string? Url { get; set; }
        public string? Targeti { get; set; }
        public bool? Aktive { get; set; }
        public string? PageName { get; set; }
        public string? PageText { get; set; }
        public int? Niveli { get; set; }
        public bool? hasChild { get; set; }
        public bool? OtherSource { get; set; }
        public bool? IsMegaMenu { get; set; }
        public bool? IsClicked { get; set; }
        public List<MenuModel> submenu { get; set; }
        public Medium media { get; set; }
    }
    public class MenuAtoZModel
    {
        public string Letter { get; set; }
        public List<MenuModel>? menus { get; set; }
    }
}
