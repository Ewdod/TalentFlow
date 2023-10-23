using ApplicationCore.Entities;
using Infrastructure.Models;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using Web.Areas.CompanyManager.Controllers;
using Web.Models;

namespace Web.Areas.Personnel.Controllers
{
    public class PersonnelMainPageController : PersonnelBaseController
    {
        private readonly ILogger<PersonnelMainPageController> _logger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TalentFlowDbContext _db;

        public PersonnelMainPageController(ILogger<PersonnelMainPageController> logger, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TalentFlowDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<IActionResult> Details(string? id)
        {
            await ViewElements();
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            ViewData["Id"] = user.Id;
            ViewData["Photo"] = Convert.ToBase64String(user.Photo);
            ViewData["Occupation"] = user.Occupation;
            ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
            return View(user);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            await ViewElements();
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);

            UserViewModel userViewModel = new UserViewModel
            {
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Photo = user.Photo != null ? new FormFile(new MemoryStream(user.Photo), 0, user.Photo.Length, "name", "fileName") : null,
            };
            string message = TempData["Message"] as string;
            if (!message.IsNullOrEmpty())
            {
                ViewData["Message"] = message;
            }
            ViewData["Id"] = user.Id;
            ViewData["Photo"] = Convert.ToBase64String(user.Photo);
            ViewData["Occupation"] = user.Occupation;
            ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,Photo,PhoneNumber,Address,PhotoByte,EndDate")] UserViewModel applicationUser, string id)
        {
            await ViewElements();
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);


            if (user == null)
                return NotFound();


            if (applicationUser.Photo != null && applicationUser.Photo.Length > 0)
            {
                string dosyaYolu = applicationUser.Photo.FileName;
                string uzanti = Path.GetExtension(dosyaYolu);

                if (uzanti != null && (uzanti.ToLower() == ".jpg" || uzanti.ToLower() == ".png" || uzanti.ToLower() == ".jpeg"))
                    user.Photo = ConvertIFormFileToByteArray(applicationUser.Photo);
                else
                {
                    ModelState.AddModelError("Photo", "We only accept .jpg, .png and .jpeg formats");
                    await ViewElements();
                    return View();
                }
            }

            if (!ModelState.IsValid)
            {
                await ViewElements();
                return View(applicationUser);
            }
            else
            {
                user.PhoneNumber = applicationUser.PhoneNumber;
            }

            user.Address = applicationUser.Address;

            await _userManager.UpdateAsync(user);
            TempData.Add("Message", "Güncelleme işlemi başarılı");
            ViewData["Id"] = user.Id;
            ViewData["Photo"] = Convert.ToBase64String(user.Photo);
            ViewData["Occupation"] = user.Occupation;
            ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
            return RedirectToAction("Edit");
        }


        public async Task<IActionResult> Index()
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            var firstLoginRecord = _db.FirstLogins.Where(fl => fl.UserId == user.Id).FirstOrDefault();
            if (firstLoginRecord != null)
            {
                ViewData["Id"] = user.Id;
                ViewData["Photo"] = Convert.ToBase64String(user.Photo);
                ViewData["Occupation"] = user.Occupation;
                ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
                return View(user);
            }
            else
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                SaveLoginTime(user.Id);
                return RedirectToAction("ResetPassword", "Account", new
                {
                    area = "Identity",
                    code = resetToken,
                    email = user.Email
                });
            }
        }
        public async Task<IActionResult> ChangePassword()
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            //SaveLoginTime(user.Id);
            return RedirectToAction("ResetPassword", "Account", new
            {
                area = "Identity",
                code = resetToken,
                email = user.Email
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task ViewElements()
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            ViewData["Id"] = userManager.Id;
            ViewData["Photo"] = Convert.ToBase64String(userManager.Photo);
            ViewData["Occupation"] = userManager.Occupation;
            ViewData["Person"] = userManager.FirstName + " " + (userManager.SecondName == null ? "" : (userManager.SecondName + " ")) + userManager.LastName + " " + (userManager.SecondLastName == null ? "" : userManager.SecondLastName);
        }
        public void SaveLoginTime(string userId)
        {
            var loginLog = new FirstLogin
            {
                UserId = userId,
                FirstLoginDate = DateTime.Now
            };
            _db.FirstLogins.Add(loginLog);
            _db.SaveChanges();
        }
    }
}
