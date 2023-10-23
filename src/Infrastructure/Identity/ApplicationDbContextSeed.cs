using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Constants;
using ApplicationCore.Enums;

namespace Infrastructure.Identity
{
    public static class ApplicationDbContextSeed
    {
        public async static Task SeedAsync(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            await db.Database.MigrateAsync();            

            if (await userManager.Users.AnyAsync() || await roleManager.Roles.AnyAsync())
                return;
            string filePath = "wwwroot/company-manager/img/logo.png";
            var companyManager = new ApplicationUser()
            {
                UserName = AuthorizationConstants.DEFAULT_ADMIN_USER,
                Email = AuthorizationConstants.DEFAULT_ADMIN_USER,
                EmailConfirmed = true,
                FirstName = "Swarley",
                LastName = "Stinson",
                BirthDate = new DateTime(1975, 4, 10),
                BirthPlace = "New York",
                IdentityNumber = "10000000146",
                PhoneNumber = "+905009998877",
                StartDate = new DateTime(2020, 1, 25),
                Status = true,
                CompanyName = "Bilgeadam Boost",
                Occupation = "Software Developer - Team Leader",
                Department = "Software Development",
                Address = "Bilgeadam Sokak No: 1 Daire: 1 Kat: 1 Çankaya / Ankara",
                Salary = 150000.00m,
                Photo = ConvertFileToByteArray(filePath),
                Gender = Gender.Male,
                AnnualLeaveAllowance = 140
            };

            var manager = new ApplicationUser()
            {
                UserName = AuthorizationConstants.DEFAULT_MANAGER,
                Email = AuthorizationConstants.DEFAULT_MANAGER,
                EmailConfirmed = true,
                FirstName = "Barney",
                LastName = "Stinson",
                BirthDate = new DateTime(1970, 4, 10),
                BirthPlace = "New York",
                IdentityNumber = "10000000146",
                PhoneNumber = "+905009998877",
                StartDate = new DateTime(2020, 1, 25),
                Status = true,
                CompanyName = "Bilgeadam",
                Occupation = "Site Manager",
                Department = "Site Management",
                Address = "Bilgeadam Sokak No: 1 Daire: 1 Kat: 1  Esenler/ İstanbul",
                Salary = 150000.00m,
                Photo = ConvertFileToByteArray(filePath),
                Gender = Gender.Male,
                AnnualLeaveAllowance = 140
            };
            await userManager.CreateAsync(companyManager, AuthorizationConstants.DEFAULT_PASSWORD);
            await userManager.CreateAsync(manager, AuthorizationConstants.DEFAULT_MANAGERPASSWORD);
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.ADMINISTRATOR));
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.PERSONNEL));

            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.MANAGER));
            await userManager.AddToRoleAsync(companyManager, AuthorizationConstants.Roles.ADMINISTRATOR);
            await userManager.AddToRoleAsync(manager, AuthorizationConstants.Roles.MANAGER);
        }

        public static byte[] ConvertFileToByteArray(string filePath)
        {
            try
            {
                // Dosya yoksa veya yol geçerli değilse null döndürün
                if (!File.Exists(filePath))
                {
                    return null;
                }

                // Dosyayı okuyun ve bir byte dizisine dönüştürün
                byte[] fileBytes = File.ReadAllBytes(filePath);
                return fileBytes;
            }
            catch (Exception ex)
            {
                // Hata durumunda ilgili işlemleri yapabilirsiniz
                Console.WriteLine("Dosya dönüştürülürken bir hata oluştu: " + ex.Message);
                return null;
            }
        }
    }
}
