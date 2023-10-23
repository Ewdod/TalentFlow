using ApplicationCore.Constants;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity;

namespace Web.Areas.Manager.Controllers
{
    public class CompanyManagerController : ManagerBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly TalentFlowDbContext _tdb;

        public CompanyManagerController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, TalentFlowDbContext tdb)
        {
            _userManager = userManager;
            _db = db;
            _tdb = tdb;
        }

        public async Task<IActionResult> CreateManager()
        {
            var lst = _tdb.Companies.ToList();
            ViewData["Companies"] = lst;
            await ViewElements();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateManager(PersonnelModel personnelModel)
        {
            ViewBag.Companies = _tdb.Companies;
            ViewData["Companies"] = _tdb.Companies;
            personnelModel.Companies = _tdb.Companies.ToList();
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            var occupationValues = Enum.GetValues(typeof(Occupation)).Cast<Occupation>();
            ViewBag.OccupationValues = occupationValues;
            var departmentValues = Enum.GetValues(typeof(Department)).Cast<Department>();
            ViewBag.DepartmentValues = departmentValues;
            bool status;
            byte[] photo;
            if (personnelModel.EndDate == null)
            {
                status = true;
            }
            else
            {
                status = false;
            }
            string filePath = "wwwroot/company-manager/img/profile.jpg";
            //if (personnelModel.Photo != null)
            //{
            //    photo = ConvertIFormFileToByteArray(personnelModel.Photo);
            //}
            //else
            //{
            //    photo = ConvertFileToByteArray(filePath);
            //}
            if (personnelModel.Photo != null && personnelModel.Photo.Length > 0)
            {
                string dosyaYolu = personnelModel.Photo.FileName;
                string uzanti = Path.GetExtension(dosyaYolu);

                if (uzanti != null && (uzanti.ToLower() == ".jpg" || uzanti.ToLower() == ".png" || uzanti.ToLower() == ".jpeg"))
                    userManager.Photo = ConvertIFormFileToByteArray(personnelModel.Photo);
                else
                {
                    ModelState.AddModelError("Photo", "We only accept .jpg, .png and .jpeg formats");
                    await ViewElements();
                    return View();
                }
            }

            var user = new ApplicationUser
            {
                UserName = ConvertTRCharToENChar(personnelModel.FirstName.ToLower()) + "." + ConvertTRCharToENChar(personnelModel.LastName.ToLower()) + $"@{ConvertTRCharToENChar(personnelModel.CompanyName.ToLower().Replace(" ", ""))}.com",
                FirstName = personnelModel.FirstName,
                SecondName = personnelModel.SecondName,
                LastName = personnelModel.LastName,
                SecondLastName = personnelModel.SecondLastName,
                Email = ConvertTRCharToENChar(personnelModel.FirstName.ToLower()) + "." + ConvertTRCharToENChar(personnelModel.LastName.ToLower()) + $"@{ConvertTRCharToENChar(personnelModel.CompanyName.ToLower().Replace(" ", ""))}.com",
                EmailConfirmed = true,
                BirthDate = personnelModel.BirthDate,
                BirthPlace = personnelModel.BirthPlace,
                IdentityNumber = personnelModel.IdentityNumber,
                PhoneNumber = personnelModel.PhoneNumber,
                StartDate = personnelModel.StartDate,
                EndDate = personnelModel.EndDate,
                CompanyName = personnelModel.CompanyName,
                Occupation = "Company Manager",
                Department = "Company Manager",
                Address = personnelModel.Address,
                Salary = personnelModel.Salary,
                Photo = userManager.Photo,
                Status = status,
                Gender = personnelModel.Gender,
                AnnualLeaveAllowance = AnnualLeaveAllowanceCalculator(personnelModel.StartDate)
            };

            if (_userManager.Users.Any(x => x.Email == user.Email))
            {
                TempData["ErrorMessagee"] = $"'{user.Email}' email address is already in use.";
                return View(personnelModel);
            }

            string randomPassword = GenerateRandomPassword(10);
            var result = await _userManager.CreateAsync(user, randomPassword);


            // TODO: bunlar kulanilabilir
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var user1 = await _userManager.ChangePasswordAsync(user,currentPassword,newPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AuthorizationConstants.Roles.ADMINISTRATOR);
                await _db.SaveChangesAsync();
                await Mail(user.Email, randomPassword);
                return RedirectToAction("CreateManager");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("ListCompanyManagers");
       
         }
        public async Task<IActionResult> ListCompanyManagers(string status)
        {
            await ViewElements();
            var users = await _userManager.GetUsersInRoleAsync(AuthorizationConstants.Roles.ADMINISTRATOR);
            var lstUsers = new List<ApplicationUser>();

            if (status == "true")
            {
                lstUsers.AddRange(users.Where(x => x.Status == true));
            }
            else if (status == "false")
            {
                lstUsers.AddRange(users.Where(x => x.Status == false));
            }
            else
            {
                lstUsers.AddRange(users);
            }
            return View(lstUsers);
        }

        public async Task<IActionResult> ListPersonnel(string status)
        {
            await ViewElements();
            var users = await _userManager.GetUsersInRoleAsync(AuthorizationConstants.Roles.PERSONNEL);
            var lstUsers = new List<ApplicationUser>();

            if (status == "true")
            {
                lstUsers.AddRange(users.Where(x => x.Status == true));
            }
            else if (status == "false")
            {
                lstUsers.AddRange(users.Where(x => x.Status == false));
            }
            else
            {
                lstUsers.AddRange(users);
            }
            return View(lstUsers);

        }

        public async Task<IActionResult> ManagerDetails(string id)
        {
            await ViewElements();
            var user = await _userManager.FindByIdAsync(id);
            // TODO: icinde sirket blgileride gonderilecek
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
                return RedirectToAction("ListCompanyManagers");
            }
            else
            {
                _db.Remove(personnel);
                await _db.SaveChangesAsync();
                await ViewElements();
                return RedirectToAction("ListCompanyManagers");
            }
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
                                     <p>Website: https://talentflowhr.azurewebsites.net/ </p>
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
    }
}
