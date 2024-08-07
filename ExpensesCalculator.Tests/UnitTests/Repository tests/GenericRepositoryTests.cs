using ExpensesCalculator.Data;
using ExpensesCalculator.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ExpensesCalculator.UnitTests
{
    public abstract class GenericRepositoryTests<T> : IDisposable where T : DbObject
    {
        protected readonly DbConnection _connection;
        protected readonly DbContextOptions<ExpensesContext> _contextOptions;

        protected readonly List<T> emptyList = new List<T>();

        public GenericRepositoryTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ExpensesContext>()
            .UseSqlite(_connection)
                .Options;

            using var context = new ExpensesContext(_contextOptions);

            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        public ExpensesContext CreateContext() => new ExpensesContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }
}
