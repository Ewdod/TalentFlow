using ApplicationCore.Entities;

namespace Web.Areas.CompanyManager.Models
{
    public class LeaveRequestViewModel
    {
        public List<LeaveRequest> LeaveRequests { get; set; } = new();
    }
}
