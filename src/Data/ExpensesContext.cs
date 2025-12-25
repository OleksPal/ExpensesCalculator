using ExpensesCalculator.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace ExpensesCalculator.Data
{
    public class ExpensesContext : IdentityDbContext
    {
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options) { }

        public DbSet<Item> Items => Set<Item>();
        public DbSet<Check> Checks => Set<Check>();
        public DbSet<DayExpenses> Days => Set<DayExpenses>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var converter = new ValueConverter<ICollection<string>, string>(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<ICollection<string>>(v, JsonSerializerOptions.Default));            

            modelBuilder.Entity<Item>()
                .Property(e => e.Users)
                .HasConversion(converter)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Item>()
                .Property(e => e.Tags)
                .HasConversion(converter)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Check>()
                .Property(e => e.Photo)
                .HasColumnType("varbinary(max)");

            modelBuilder.Entity<DayExpenses>()
                .Property(e => e.Participants)
                .HasConversion(converter)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<DayExpenses>()
                .Property(e => e.PeopleWithAccess)
                .HasConversion(converter)
                .HasColumnType("nvarchar(max)");
        }
    }
}
