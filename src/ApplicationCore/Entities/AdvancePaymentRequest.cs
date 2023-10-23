using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class AdvancePaymentRequest : BaseEntity
    {
        public string PersonnelId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SecondFirstName { get; set; } 
        public string? SecondLastName { get; set; }
        public string Advance { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ResponseDate { get; set; } // Nullable, henüz cevaplanmamışsa null olabilir
        public Status Status { get; set; } = Status.Pending; // Onay Bekliyor, Kabul Edildi, Reddedildi
        public AdvancePaymentType Type { get; set; } // Kurumsal veya Bireysel

    }
}
