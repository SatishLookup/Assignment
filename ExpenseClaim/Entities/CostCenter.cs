using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ExpenseClaim.Entities
{
    public class CostCenter
    {
        [Key]
        public string CostCenterId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CostCenterName { get; set; }

        public string IsDefault { get; set; } = "N";

        public ICollection<Expenses> Expenses { get; set; }
    }
}
