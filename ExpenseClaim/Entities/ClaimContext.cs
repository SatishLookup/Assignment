using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ExpenseClaim.Entities
{
    public class ClaimContext: DbContext
    {
        public ClaimContext(DbContextOptions<ClaimContext> options)
            :base(options)
        {
            //Database.Migrate();
        }

        public DbSet<Expenses> Expenses { get; set; }
        public DbSet<CostCenter> CostCenter { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CostCenter>()
           .HasMany(e => e.Expenses)
           .WithOne(c => c.CostCenter);

        }
    }

    
}
