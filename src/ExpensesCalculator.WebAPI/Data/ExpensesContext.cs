using ExpensesCalculator.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.WebAPI.Data
{
    public class ExpensesContext : DbContext
    {
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options) { }

        public DbSet<Item> Items => Set<Item>();
        public DbSet<Check> Checks => Set<Check>();
        public DbSet<DayExpenses> Days => Set<DayExpenses>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);       

            modelBuilder.Entity<Item>()
                .Property(e => e.Users)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Item>()
                .Property(e => e.Tags)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Check>()
                .Property(e => e.Photo)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<DayExpenses>()
                .Property(e => e.Participants)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<DayExpenses>()
                .Property(e => e.PeopleWithAccess)
                .HasColumnType("nvarchar(max)");
        }
    }
}
