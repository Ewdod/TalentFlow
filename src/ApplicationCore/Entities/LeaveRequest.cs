using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class LeaveRequest : BaseEntity
    {       
        public string PersonnelId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? SecondFirstName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public int NumberOfDays
        {
            get
            {
                TimeSpan span = EndDate - StartDate;
                return span.Days;
            }
        }
        public Status Status { get; set; } = Status.Pending;
        public DateTime? ResponseDate { get; set; } 
        public byte[]? DocumentPath { get; set; }
        public LeaveType Type { get; set; }



    }
}
