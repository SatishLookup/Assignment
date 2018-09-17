using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseClaim.Entities;
using ExpenseClaim.Services.Contract;

namespace ExpenseClaim.Services
{
    public class ClaimRepository : IClaimRepository
    {
        private ClaimContext _context;

        public ClaimRepository(ClaimContext context)
        {
            _context = context;
        }

        public Expenses GetExpense(Guid id)
        {
            return _context.Expenses.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Expenses> GetExpenseForCostCenter(string costcenterId)
        {
            return _context.Expenses.Where(a => a.CostCenterId == costcenterId);
        }

        public IEnumerable<Expenses> GetExpenses()
        {
            return _context.Expenses;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void AddClaim(Expenses expense)
        {
            expense.Id = Guid.NewGuid();
            _context.Expenses.Add(expense);
        }

        public string getDefaultCostCenter()
        {
            return _context.CostCenter.FirstOrDefault().CostCenterId;
        }
    }
}
