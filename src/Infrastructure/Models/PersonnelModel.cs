using ApplicationCore.Entities;
using ApplicationCore.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class PersonnelModel
    {
        public IFormFile? Photo { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [MaxLength(50)]
        public string? SecondName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; } = null!;
        [MaxLength(50)]
        public string? SecondLastName { get; set; }
        // Annotation
        public DateTime BirthDate { get; set; }
        [MaxLength(50)]
        public string BirthPlace { get; set; } = null!;
        public string IdentityNumber { get; set; } = null!;
        // Annotation
        public DateTime StartDate { get; set; }

        //Annotation
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }
        [MaxLength(50)]
        public string CompanyName { get; set; } = null!;
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;
        [MaxLength(50)]
        public string Department { get; set; } = null!;
        public string Address { get; set; } = null!;
        [Range(typeof(decimal), "11402", "999999")]
        public decimal Salary { get; set; }
        public Gender? Gender { get; set; }

        [RegularExpression(@"^\+90\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number in the format +90xxxxxxxxxx.")]
        [MaxLength(13)]
        public string? PhoneNumber { get; set; }

        public List<Company> Companies { get; set; } = new();
    }
}
