using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class ExpenseTypeData
    {
        public static List<ExpenseTypeInfo> ExpenseTypes { get; } = new List<ExpenseTypeInfo>
        {
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Transportation,
                MinExpense = 200,
                MaxExpense = 3000,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Accommodation,
                MinExpense = 200,
                MaxExpense = 10000,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Travel,
                MinExpense = 200,
                MaxExpense = 1000,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Dining,
                MinExpense = 300,
                MaxExpense = 2000,
                RequiresDocument = true,

            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Internet_Communication_Fees,
                MinExpense = 100,
                MaxExpense = 200,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Meeting_Conference_Fees,
                MinExpense = 500,
                MaxExpense = 10000,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Office_Supplies,
                MinExpense = 200,
                MaxExpense = 400,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Technology_Expenses,
                MinExpense = 200,
                MaxExpense = 10000,
                RequiresDocument = true,
            },
            new ExpenseTypeInfo
            {
                Type = ExpenseType.Health_Insurance_Expenses,
                MinExpense = 500,
                MaxExpense = 20000,
                RequiresDocument = true,
            }
        };
    }
}