using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class ExpenseRequest : BaseEntity
    {
        public string PersonnelId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? SecondFirstName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public string Expense { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public Status Status { get; set; } = Status.Pending;
        public DateTime? ResponseDate { get; set; }
        public byte[]? DocumentPath { get; set; }
        public ExpenseType Type { get; set; }
    }
}
