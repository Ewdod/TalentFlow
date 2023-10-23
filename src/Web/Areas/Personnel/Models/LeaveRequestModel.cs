using ApplicationCore.Entities;
using ApplicationCore.Enums;
using Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Personnel.Models
{
    public class LeaveRequestModel
    {
        [Required(ErrorMessage = "Please choose type of leave request.")]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage = "Please enter a start date.")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter an end date.")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Document")]
        //[FileExtensions(Extensions = ".pdf,.doc,.docx", ErrorMessage = "Please choose a valid file format (.pdf, .docx, .docx).")]
        public IFormFile? Document { get; set; }
    }
}
