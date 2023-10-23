namespace Web.Areas.CompanyManager.Models
{
    public class ListPersonnelViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new();
        public PaginationInfoViewModel PaginationInfo { get; set; } = null!;
    }
}
