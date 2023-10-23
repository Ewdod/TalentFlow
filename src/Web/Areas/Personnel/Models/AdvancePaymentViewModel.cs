using ApplicationCore.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Web.Areas.Personnel.Models
{
    public class AdvancePaymentViewModel
    {
        public string? PersonnelId { get; set; }

        [Required(ErrorMessage = "Advance type is required.")]
        public AdvancePaymentType AdvanceType { get; set; }

        [Required(ErrorMessage = "Advance amount is required.")]
        public decimal Advance { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = null!;

        public DateTime? RequestDate { get; set; } = DateTime.Now;            
    }
}