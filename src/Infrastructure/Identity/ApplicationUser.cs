using ApplicationCore.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // default photo atamasi yapilabilir
        //string filePath = "wwwroot/site-manager/img/logo.png"; 
        //byte[] imageBytes = ConvertFileToByteArray(filePath);

        public byte[]? Photo { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "FirstName can not be empty!")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string? SecondName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "LastName can not be empty!")]
        public string LastName { get; set; } = null!;

        [MaxLength(50)]
        public string? SecondLastName { get; set; }
        // Annotation

        [Required(ErrorMessage = "BirthDate can not be empty!")]
        public DateTime BirthDate { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "BirthPlace can not be empty!")]
        public string BirthPlace { get; set; } = null!;

        [Required(ErrorMessage = "IdentityNumber can not be empty!")]
        public string IdentityNumber { get; set; } = null!;
        // Annotation
        public DateTime StartDate { get; set; }
        //Annotation
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Company Name can not be empty!")]
        public string CompanyName { get; set; } = null!;

        [MaxLength(50)]
        [Required(ErrorMessage = "Occupation can not be empty!")]
        public string Occupation { get; set; } = null!;

        [MaxLength(50)]
        [Required(ErrorMessage = "Department can not be empty!")]
        public string Department { get; set; } = null!;

        [Required(ErrorMessage = "Address can not be empty!")]
        public string Address { get; set; } = null!;

        //[Range(typeof(decimal), "11500", "999999")]
        [Range(11402, 999999, ErrorMessage = "Salary must be between 11402 and 999999.")]
        public decimal Salary { get; set; }

        public Gender? Gender { get; set; }

        public int? AnnualLeaveAllowance { get; set; }
        public double? AdvancePayment { get; set; }
    }
}
