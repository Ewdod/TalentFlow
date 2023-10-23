global using Infrastructure.Identity;
global using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using ApplicationCore.Interfaces;

namespace Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionStringIdentity = builder.Configuration.GetConnectionString("TalentFlowIdentityDb") ?? throw new InvalidOperationException("Connection string 'TalentFlowIdentityDb' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionStringIdentity));
            var connectionStringMain = builder.Configuration.GetConnectionString("TalentFlowDb") ?? throw new InvalidOperationException("Connection string 'TalentFlowDb' not found.");
            builder.Services.AddDbContext<TalentFlowDbContext>(options =>
                options.UseSqlServer(connectionStringMain));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddSession();
                    
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddControllersWithViews().AddRazorPagesOptions(options => {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            });
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=LoginPage}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var appIdentityDb = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await ApplicationDbContextSeed.SeedAsync(appIdentityDb, roleManager, userManager);
            }

            app.Run();
        }
    }
}