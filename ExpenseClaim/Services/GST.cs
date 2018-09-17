using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseClaim.Services.Contract;

namespace ExpenseClaim.Services
{
    public class GST : IGST
    {
        const decimal gstPer = 0.1m;

        public decimal CalculateGST(decimal amount)
        {
            return amount * gstPer;
        }
    }
}
