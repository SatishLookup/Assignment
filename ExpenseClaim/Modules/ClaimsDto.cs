using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseClaim.Modules
{
    public class ClaimsDto
    {
        public string Id {get;set;}

        [Required]
        public string costCenter { get; set; }


        [Required]
        [Range(1.00, 9999.99)]
        public string total { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public string Vendor { get; set; }

        public string Description { get; set; }
        public decimal gstCalculated { get; set; }

        [Required]
        public DateTime date { get; set; }
    }
}
