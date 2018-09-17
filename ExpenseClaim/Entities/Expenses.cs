using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseClaim.Entities
{
    public class Expenses
    {
        [Key]
        public Guid Id { get; set; }

        //[Required]
        public string CostCenterId { get; set; }

        //[Required]
        public decimal TotalAmount { get; set; }

        //[Required]
        public decimal Gst { get; set; }

        //[Required]
        public DateTime TransactionDate { get; set; }

        //[Required]
        public DateTime CreatedAt { get; set; }

        public string Vendor { get; set; }

        public string Description { get; set; }

        //[Required]
        public string PaymentMethod { get; set; }

        public CostCenter CostCenter { get; set; }
    }
}
