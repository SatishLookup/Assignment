using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseClaim.Entities
{
    public static class ClaimExtensions
    {
        public static void EnsureSeedDataForClaim(this ClaimContext context)
        {
            //Initialise CostCenter db
            context.CostCenter.RemoveRange(context.CostCenter);
            context.SaveChanges();

            //init seed data
            var costCenter = new List<CostCenter>()
            {
                new CostCenter(){CostCenterId = "UNKNOWN", CostCenterName = "Unknown CostCenter", IsDefault="Y" },
                new CostCenter(){CostCenterId = "DEV002", CostCenterName = "Development CostCenter" },
                new CostCenter(){CostCenterId = "UAT002", CostCenterName = "User Acceptance Testing CostCenter" },
                new CostCenter(){CostCenterId = "STG002", CostCenterName = "Staging CostCenter" },
                new CostCenter(){CostCenterId = "INT002", CostCenterName = "Integration Testing CostCenter" }
            };
            context.AddRange(costCenter);
            context.SaveChanges();

            //Initialise Expenses db
            context.Expenses.RemoveRange(context.Expenses);
            context.SaveChanges();

            //init seed data
            var expenses = new List<Expenses>()
            {
                new Expenses(){ Id= Guid.NewGuid(), CostCenterId = "DEV002", TotalAmount = 1000, Gst= 100,
                                TransactionDate = DateTime.Parse("1 jan 2018"), Description="Cost for DEv",
                                Vendor = "Vendor1", CreatedAt= DateTime.Parse("12 Sep 2018"), PaymentMethod="CARD"  },

                new Expenses(){ Id= Guid.NewGuid(), CostCenterId = "DEV002", TotalAmount = 2000, Gst= 200,
                                TransactionDate = DateTime.Parse("10 jan 2018"), Description = "Cost for Dev2",
                                Vendor = "Vendor2",  CreatedAt= DateTime.Parse("12 Sep 2018"), PaymentMethod = "CASH"  }
            };
            context.AddRange(expenses);
            context.SaveChanges();

        }
    }
}
