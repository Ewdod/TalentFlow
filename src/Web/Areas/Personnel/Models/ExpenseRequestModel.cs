using ApplicationCore.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Personnel.Models
{
    public class ExpenseRequestModel
    {
        [Required(ErrorMessage = "Please choose type of leave request.")]
        public ExpenseType ExpenseType { get; set; }

        [Required(ErrorMessage = "Please enter a value for expense request.")]
        public decimal Expense { get; set; }

        [Required(ErrorMessage = "Please select a currency.")]
        public string Currency { get; set; }

        [Display(Name = "Document")]
        [Required(ErrorMessage = "Please submit your document.")]
        public IFormFile Document { get; set; } = null!;
    }
}
