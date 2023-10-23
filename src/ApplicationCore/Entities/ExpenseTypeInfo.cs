using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class ExpenseTypeInfo
    {
        public ExpenseType Type { get; set; }
   
        public decimal MaxExpense { get; set; }
        public bool RequiresDocument { get; set; }
        public decimal MinExpense { get; set; }
        public List<ExpenseRequest> ExpenseRequests { get; set; } = new();
    }
}
