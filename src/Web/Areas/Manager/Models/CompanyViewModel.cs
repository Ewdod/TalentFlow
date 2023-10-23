using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Manager.Models
{
    public class CompanyViewModel
    {
        //public string OwnerId { get; set; } = null!;
        public List<string> ManagerIds { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string CompanyType { get; set; } = null!; //Ünvan(LTD, ŞTİ, A.Ş.gibi)
        public double TaxNumber { get; set; }
        public string TaxCompany { get; set; } = null!;
        public IFormFile? Logo { get; set; }

        [RegularExpression(@"^\+90\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number in the format +90xxxxxxxxxx.")]
        [MaxLength(13)]
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Employee Count must be a non-negative number.")]
        public int EmployeeCount { get; set; }
        public DateTime FoundationYear { get; set; }
        public DateTime ContractBeginningDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public bool Status { get; set; }
    }
}
