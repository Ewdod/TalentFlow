using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Web.Areas.CompanyManager.Controllers;
using Web.Models;

namespace Web.Areas.Manager.Controllers
{
    public class ManagerMainPageController : ManagerBaseController
    {
        private readonly ILogger<MainPageController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManagerMainPageController(ILogger<MainPageController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var userName = HttpContext.User.Identity.Name;
            var user = _userManager.FindByNameAsync(userName).Result;
            ViewData["Id"] = user.Id;
            ViewData["Photo"] = Convert.ToBase64String(user.Photo);
            ViewData["Occupation"] = user.Occupation;
            ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
            return View(user);
        }

        public async Task<IActionResult> Details(string? id)
        {
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
            TempData.Add("Message", "Succesfully updated.");
            ViewData["Id"] = user.Id;
            ViewData["Photo"] = Convert.ToBase64String(user.Photo);
            ViewData["Occupation"] = user.Occupation;
            ViewData["Person"] = user.FirstName + " " + (user.SecondName == null ? "" : (user.SecondName + " ")) + user.LastName + " " + (user.SecondLastName == null ? "" : user.SecondLastName);
            return RedirectToAction("Edit");
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
                Console.WriteLine("Dosya dönüştürülürken bir hata oluştu: " + ex.Message);
                return null;
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
    }
}
