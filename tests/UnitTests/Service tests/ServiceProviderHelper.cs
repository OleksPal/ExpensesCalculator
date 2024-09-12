using ExpensesCalculator.Data;
using ExpensesCalculator.Repositories;
using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpensesCalculator.UnitTests
{
    public static class Helper
    {
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            #region Repositories
            services.AddScoped<IItemRepository, ItemRepository>()
                .AddScoped<ICheckRepository, CheckRepository>()
                .AddScoped<IDayExpensesRepository, DayExpensesRepository>()
                .AddScoped<IUserRepository, UserRepository>();
            #endregion

            #region Services
            services.AddScoped<IItemService, ItemService>()
                .AddScoped<ICheckService, CheckService>()
                .AddScoped<IDayExpensesService, DayExpensesService>();
            #endregion

            services.AddDbContext<ExpensesContext>(o => o.UseInMemoryDatabase("TestDB"));

            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            var requiredService = provider.GetRequiredService<T>();

            var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpensesContext>();

            AddData(context);

            return requiredService;
        }

        private static void AddData(ExpensesContext context)
        {
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }
    }
}
