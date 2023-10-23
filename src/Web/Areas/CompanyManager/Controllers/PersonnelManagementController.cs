using ApplicationCore.Constants;
using Infrastructure.Identity;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using Web.Areas.CompanyManager.Models;
using System.Globalization;
using System.Text;
using NuGet.Versioning;
using ApplicationCore.Enums;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Areas.CompanyManager.Controllers
{
    public class PersonnelManagementController : CompanyManagerBaseController
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonnelManagementController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PersonnelManagementController(ApplicationDbContext db, ILogger<PersonnelManagementController> logger, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> ListPersonnel()
        {
            await ViewElements();
            var personnelUsers = (await _userManager.GetUsersInRoleAsync("Personnel")).Where(s => s.Status == true).ToList();
            var activePersonelViewModel = new ListPersonnelViewModel();
            activePersonelViewModel.Users = personnelUsers;
            return View(activePersonelViewModel);
        }

        public async Task<IActionResult> ListPassivePersonnel()
        {
            await ViewElements();
            var personnelUsers = (await _userManager.GetUsersInRoleAsync("Personnel")).Where(s => s.Status == false).ToList();
            var passivePersonelViewModel = new ListPersonnelViewModel();
            passivePersonelViewModel.Users = personnelUsers;
            return View("ListPersonnel", passivePersonelViewModel);
        }

        public async Task<IActionResult> CreatePersonnel()
        {
            await ViewElements();
            var occupationValues = Enum.GetValues(typeof(Occupation)).Cast<Occupation>();
            ViewBag.OccupationValues = occupationValues;
            var departmentValues = Enum.GetValues(typeof(Department)).Cast<Department>();
            ViewBag.DepartmentValues = departmentValues;

            TempData["Message"] = "Personnel created successfully";
            TempData["MessageShown"] = null;
            var user = new PersonnelModel();
            user.Photo = null;
            var userName = HttpContext.User.Identity?.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var personnel = await _userManager.Users.FirstAsync(m => m.Id == id);

            if (personnel == null)
            {
                return NotFound();
            }
            if (personnel.Status = true)
            {
                personnel.Status = false;
                await _db.SaveChangesAsync();
                await ViewElements();
                return RedirectToAction("ListPersonnel");
            }
            else
            {
                _db.Remove(personnel);
                await _db.SaveChangesAsync();
                await ViewElements();
                return RedirectToAction("ListPersonnel");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePersonnel([Bind("Id,FirstName,LastName,SecondName,SecondLastName,BirthDate,BirthPlace,IdentityNumber,StartDate,EndDate,CompanyName,Occupation, Department,Salary,Photo,PhoneNumber,Address,Gender")] PersonnelModel personnel)
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            var occupationValues = Enum.GetValues(typeof(Occupation)).Cast<Occupation>();
            ViewBag.OccupationValues = occupationValues;
            var departmentValues = Enum.GetValues(typeof(Department)).Cast<Department>();
            ViewBag.DepartmentValues = departmentValues;
            bool status;
            byte[] photo;
            if (personnel.EndDate == null)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            string filePath = "wwwroot/company-manager/img/profile.jpg";
            if (personnel.Photo != null && personnel.Photo.Length > 0)
            {
                string dosyaYolu = personnel.Photo.FileName;
                string uzanti = Path.GetExtension(dosyaYolu);

                if (uzanti != null && (uzanti.ToLower() == ".jpg" || uzanti.ToLower() == ".png" || uzanti.ToLower() == ".jpeg"))
                    userManager.Photo = ConvertIFormFileToByteArray(personnel.Photo);
                else
                {
                    ModelState.AddModelError("Photo", "We only accept .jpg, .png and .jpeg formats");
                    await ViewElements();
                    return View();
                }
            }

            var user = new ApplicationUser
            {
                UserName = ConvertTRCharToENChar(personnel.FirstName.ToLower()) + "." + ConvertTRCharToENChar(personnel.LastName.ToLower()) + "@bilgeadamboost.com",
                FirstName = personnel.FirstName,
                SecondName = personnel.SecondName,
                LastName = personnel.LastName,
                SecondLastName = personnel.SecondLastName,
                Email = ConvertTRCharToENChar(personnel.FirstName.ToLower()) + "." + ConvertTRCharToENChar(personnel.LastName.ToLower()) + "@bilgeadamboost.com",
                EmailConfirmed = true,
                BirthDate = personnel.BirthDate,
                BirthPlace = personnel.BirthPlace,
                IdentityNumber = personnel.IdentityNumber,
                PhoneNumber = personnel.PhoneNumber,
                StartDate = personnel.StartDate,
                EndDate = personnel.EndDate,
                CompanyName = userManager.CompanyName,
                Occupation = personnel.Occupation.Replace("_"," "),
                Department = personnel.Department.Replace("_", " "),
                Address = personnel.Address,
                Salary = personnel.Salary,
                Photo = userManager.Photo,
                Status = status,
                Gender = personnel.Gender,
                AnnualLeaveAllowance = AnnualLeaveAllowanceCalculator(personnel.StartDate)
            };

            if (_userManager.Users.Any(x => x.Email == user.Email))
            {
                ViewData["ErrorMessagee"] = $"'{user.Email}' email address is already in use.";
                return View(personnel);
            }

            //var userName = HttpContext.User.Identity.Name;
            //var userManager = await _userManager.FindByNameAsync(userName);
            //ViewData["Id"] = userManager.Id;
            //ViewData["Photo"] = Convert.ToBase64String(userManager.Photo);
            //ViewData["Occupation"] = userManager.Occupation;
            //ViewData["Person"] = userManager.FirstName + " " + (userManager.SecondName == null ? "" : (userManager.SecondName + " ")) + userManager.LastName + " " + (userManager.SecondLastName == null ? "" : userManager.SecondLastName);

            string randomPassword = GenerateRandomPassword(10);
            var result = await _userManager.CreateAsync(user, randomPassword);


            // TODO: bunlar kulanilabilir
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var user1 = await _userManager.ChangePasswordAsync(user,currentPassword,newPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AuthorizationConstants.Roles.PERSONNEL);
                await _db.SaveChangesAsync();
                await Mail(user.Email, randomPassword);
                return RedirectToAction("CreatePersonnel");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        public static byte[] ConvertFileToByteArray(string filePath)
        {
            try
            {
                // Dosya yoksa veya yol geçerli değilse null döndürün
                if (!System.IO.File.Exists(filePath))
                {
                    return null;
                }

                // Dosyayı okuyun ve bir byte dizisine dönüştürün
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return fileBytes;
            }
            catch (Exception ex)
            {
                // Hata durumunda ilgili işlemleri yapabilirsiniz
                Console.WriteLine("Error during converting the file: " + ex.Message);
                return null;
            }
        }
        public byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static string GenerateRandomPassword(int length = 12)
        {
            const string upperCases = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lowerCases = "abcdefghijkmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";
            var random = new Random();
            var passwordChars = new char[length];

            // Büyük harf, küçük harf, rakam ve özel karakterler içeren bir şifre oluşturmak için her birinden bir karakter alın.
            passwordChars[0] = upperCases[random.Next(0, upperCases.Length)];
            passwordChars[1] = lowerCases[random.Next(0, lowerCases.Length)];
            passwordChars[2] = numbers[random.Next(0, numbers.Length)];
            passwordChars[3] = specialChars[random.Next(0, specialChars.Length)];

            for (int i = 4; i < length; i++)
            {
                string allChars = upperCases + lowerCases + numbers + specialChars;
                passwordChars[i] = allChars[random.Next(0, allChars.Length)];
            }

            // Şifreyi karıştır
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(i, length);
                char temp = passwordChars[i];
                passwordChars[i] = passwordChars[randomIndex];
                passwordChars[randomIndex] = temp;
            }

            return new string(passwordChars);
        }

        public async Task ViewElements()
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            ViewData["CompanyName"] = userManager.CompanyName;
            ViewData["Id"] = userManager.Id;
            ViewData["Photo"] = Convert.ToBase64String(userManager.Photo);
            ViewData["Occupation"] = userManager.Occupation;
            ViewData["Person"] = userManager.FirstName + " " + (userManager.SecondName == null ? "" : (userManager.SecondName + " ")) + userManager.LastName + " " + (userManager.SecondLastName == null ? "" : userManager.SecondLastName);
        }

        public async Task Mail(string mail, string password)
        {
            //hedvdcjqbiipqrio
            //bu gelen maili gönder !

            try
            {
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();

                string sender = "talentflowhumanr@gmail.com";

                mailMessage.From = new MailAddress(sender);  //gönderen belirtiliyor
                mailMessage.To.Add($"{mail}");  //alıcı/lar belirtiliyor
                string subject = "Your subscription has been made successfully to TalentFlow";
                string htmlBody = $@"
                                 <html>
                                 <body>
                                     <p>Hello,</p>
                                     <p>Your company manager has added you to the TalentFlow HR system. To log in, please use the following credentials:</p>
                                     <ul>
                                         <li>Email: {mail}</li>
                                         <li>Password: {password}</li>
                                     </ul>
                                     <p>Website: https://talentflow.azurewebsites.net/ </p>
                                     <p>Thank you for using TalentFlow!</p>
                                     <img src='~/company-manager/img/logo.png' alt='TalentFlow Logo' />
                                 </body>
                                 </html>
                             ";

                mailMessage.Subject = subject;   //konu belirtiliyor
                mailMessage.Body = htmlBody;   //içerik belirtiliyor
                mailMessage.IsBodyHtml = true;

               
                smtpClient.Port = 587;   //sabit
                smtpClient.Host = "smtp.gmail.com";   //sabit
                smtpClient.EnableSsl = true;   //true olacak


                //Gönderen ve 2 factor authenticationdaki şifre yazılacak 
                //Mail adresi şifreniz DEĞİL !
                smtpClient.Credentials = new NetworkCredential(sender, "ylwytayknsdpmyqa");

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;  //sabit
                await smtpClient.SendMailAsync(mailMessage);  //gönderme metodu


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public string ConvertTRCharToENChar(string text)
        {
            return String.Join("", text.Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }

        public int AnnualLeaveAllowanceCalculator(DateTime startDate)
        {
            if (DateTime.Now.Year - startDate.Year < 1)
                return 0;
            else if (DateTime.Now.Year - startDate.Year >= 1 && DateTime.Now.Year - startDate.Year < 6)
                return 14;
            else
                return 28;
        }
    }
}
