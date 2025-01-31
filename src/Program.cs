using ExpensesCalculator.Data;
using ExpensesCalculator.Repositories;
using ExpensesCalculator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using ExpensesCalculator.Repositories.Interfaces;
using System.Globalization;

namespace ExpensesCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("uk-UA")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedUICultures = supportedCultures;
            });

            #region Repositories
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<ICheckRepository, CheckRepository>();
            builder.Services.AddScoped<IDayExpensesRepository, DayExpensesRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ICheckService, CheckService>();
            builder.Services.AddScoped<IDayExpensesService, DayExpensesService>();
            #endregion

            builder.Services.AddDbContext<ExpensesContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ExpensesContext>();

            var app = builder.Build();

            app.UseRequestLocalization();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.CreateDbIfNotExists();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
