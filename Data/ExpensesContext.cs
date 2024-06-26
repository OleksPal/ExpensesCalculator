﻿using Microsoft.EntityFrameworkCore;
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
    }
}
