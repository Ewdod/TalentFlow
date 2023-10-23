using ApplicationCore.Entities;
using Infrastructure.Data;

namespace Web.Areas.CompanyManager.Models
{
    public class ExpenseRequestViewModel
    {
        public List<ExpenseRequest> Expenses { get; set; } = new List<ExpenseRequest>();  
    }
}
