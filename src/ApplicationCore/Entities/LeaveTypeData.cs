using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class LeaveTypeData : BaseEntity
    {
        public static List<LeaveTypeInfo> LeaveTypes { get; } = new List<LeaveTypeInfo>
    {
            new LeaveTypeInfo
            {
                Type = LeaveType.Annual_Leave,
                MinYearsOfWork = 0,
                MaxYearsOfWork = 1,
                DurationInDays = 0,
                Conditions = "less than 1 year of service",
                RequiresDocument = false,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Annual_Leave,
                MinYearsOfWork = 1,
                MaxYearsOfWork = 6,
                DurationInDays = 14,
                Conditions = "1-5 years of service",
                RequiresDocument = false,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Annual_Leave,
                MinYearsOfWork = 6,
                MaxYearsOfWork = int.MaxValue,
                DurationInDays = 28,
                Conditions = "more than 5 years of service",
                RequiresDocument = false,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Free_Leave,
                RequiresDocument = false,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Maternity_Leave,
                DurationInDays = 180,
                Conditions = "For female employees with pregnancy certificate",
                RequiresDocument = true,
                Gender = Gender.Female
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Paternity_Leave,
                DurationInDays = 5,
                Conditions = "For male employees with birth certificate",
                RequiresDocument = true,
                Gender = Gender.Male
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Sick_Leave,
                DurationInDays = 2,
                Conditions = "With doctor's certificate (up to 20 days with health committee approval)",
                RequiresDocument = true,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Bereavement_Leave,
                DurationInDays = 3,
                Conditions = "For the loss of a first-degree relative with death certificate",
                RequiresDocument = true,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Marriage_Leave,
                DurationInDays = 3,
                Conditions = "For marriage",
                RequiresDocument = true,
                Gender = Gender.None
            },
            new LeaveTypeInfo
            {
                Type = LeaveType.Companion_Leave,
                DurationInDays = 3,
                Conditions = "For companion purposes",
                RequiresDocument = false,
                Gender = Gender.None
            },
        };
    }
}
