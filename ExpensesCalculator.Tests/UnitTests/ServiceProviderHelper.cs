using ExpensesCalculator.Repositories.Interfaces;
using ExpensesCalculator.Repositories;
using ExpensesCalculator.Services;
using Microsoft.Extensions.DependencyInjection;
using ExpensesCalculator.Data;
using Microsoft.EntityFrameworkCore;

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

            services.AddDbContext<ExpensesContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            return provider.GetRequiredService<T>();
        }
    }
}
