using ApplicationCore.Entities;

namespace Web.Areas.Personnel.Models
{
    public class ListAllRequestsViewModel
    {
        public List<AdvancePaymentRequest> AdvancePaymentRequests { get; set; }
        public List<ExpenseRequest> ExpenseRequests { get; set; }
        public List<LeaveRequest> LeaveRequests { get; set; }
        public List<AdvancePaymentRequest> AdvanceRequests { get; internal set; }
    }
}
