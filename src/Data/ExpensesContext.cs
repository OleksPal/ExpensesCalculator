using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ExpensesCalculator.Models;

namespace ExpensesCalculator.Data
{
    public class ExpensesContext : IdentityDbContext
    {
        public ExpensesContext(DbContextOptions<ExpensesContext> options) : base(options) { }

        public DbSet<Item> Items => Set<Item>();
        public DbSet<Check> Checks => Set<Check>();
        public DbSet<DayExpenses> Days => Set<DayExpenses>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<ItemTag> ItemTags => Set<ItemTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });

            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Item)
                .WithMany(i => i.ItemTags)
                .HasForeignKey(it => it.ItemId);

            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId);
        }
    }
}
