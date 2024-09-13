using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpensesCalculator.Data
{
    public class ExpensesContext : IdentityDbContext
    {
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<DayExpenses> Days { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole<int>> roles = new List<IdentityRole<int>>
            {
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<int> {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER"
                }
            };

            builder.Entity<IdentityRole<int>>().HasData(roles);
        }
    }
}
