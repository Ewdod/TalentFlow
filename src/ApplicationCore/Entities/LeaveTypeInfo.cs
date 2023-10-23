using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class LeaveTypeInfo : BaseEntity
    {
        public LeaveType Type { get; set; }
        public int MinYearsOfWork { get; set; }
        public int MaxYearsOfWork { get; set; }
        public int? DurationInDays { get; set; }
        public string? Conditions { get; set; }
        public bool RequiresDocument { get; set; }
        public Gender Gender { get; set; }

        public List<LeaveRequest> LeaveRequests { get; set; } = new();
    }
}
