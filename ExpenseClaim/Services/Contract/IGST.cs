using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseClaim.Services.Contract
{
    public interface IGST
    {
        decimal CalculateGST(decimal amount);
    }
}
