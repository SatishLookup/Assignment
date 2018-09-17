using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseClaim.Entities;

namespace ExpenseClaim.Services.Contract
{
    public interface IClaimRepository
    {
        IEnumerable<Expenses> GetExpenses();
        Expenses GetExpense(Guid id);
        IEnumerable<Expenses> GetExpenseForCostCenter( string costcenterId);
        bool Save();
        void AddClaim(Expenses expense);
        string getDefaultCostCenter();

    }
}
