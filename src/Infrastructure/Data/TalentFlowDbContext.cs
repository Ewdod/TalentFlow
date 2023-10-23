using ApplicationCore.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;

namespace Infrastructure.Data
{
    public class TalentFlowDbContext : DbContext
    {
        public TalentFlowDbContext(DbContextOptions<TalentFlowDbContext> options) : base(options)
        {  
            
        }
        
        public DbSet<LeaveRequest> LeaveRequests { get; set; }    
        public DbSet<ExpenseRequest> ExpenseRequests { get; set; }
        public DbSet<AdvancePaymentRequest> AdvancePaymentRequests { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<FirstLogin> FirstLogins { get; set; }
    }
}
