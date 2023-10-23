using ApplicationCore.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class PersonnelViewModel : IdentityUser
    {
        public byte[]? Photo { get; set; }
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
        public string Gender { get; set; } = null!;
        [MaxLength(50)]
        public string CompanyName { get; set; } = null!;
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;
        [MaxLength(50)]
        public string Department { get; set; } = null!;
        public string Address { get; set; } = null!;
        [Range(typeof(decimal), "11402", "999999")]
        public decimal Salary { get; set; }

        public ICollection<LeaveRequest>? LeaveRequests { get; set; }
    }
}
