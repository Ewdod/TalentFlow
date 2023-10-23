using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Company : BaseEntity
    {
        public string OwnerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string CompanyType { get; set; } = null!; //Ünvan(LTD, ŞTİ, A.Ş.gibi)
        public double TaxNumber { get; set; } 
        public string TaxCompany { get; set;} = null!; // = $"0{TaxNumber}16"
        public byte[]? Logo { get;set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int EmployeeCount { get; set; } 
        public DateTime FoundationYear { get; set; }
        public DateTime ContractBeginningDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public bool Status { get; set; }    
    }
}

