using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using Web.Areas.CompanyManager.Models;
using Web.Areas.Manager.Models;
using static iTextSharp.text.pdf.AcroFields;

namespace Web.Areas.Manager.Controllers
{
    public class CompanyManagementController : ManagerBaseController
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyManagementController(IRepository<Company> companyRepository, UserManager<ApplicationUser> userManager)
        {
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> CreateCompany()
        {
            await ViewElements();
            ViewData["OwnerList"] = await _userManager.GetUsersInRoleAsync(AuthorizationConstants.Roles.ADMINISTRATOR);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CompanyViewModel companyModel)
        {
            var userName = HttpContext.User.Identity.Name;
            var userManager = await _userManager.FindByNameAsync(userName);
            byte[] photo;
            bool status = false;
            ViewData["OwnerList"] = await _userManager.GetUsersInRoleAsync(AuthorizationConstants.Roles.ADMINISTRATOR);
            string filePath = "wwwroot/company-manager/img/profile.jpg";
            if (companyModel.Logo != null && companyModel.Logo.Length > 0)
            {
                string dosyaYolu = companyModel.Logo.FileName;
                string uzanti = Path.GetExtension(dosyaYolu);

                if (uzanti != null && (uzanti.ToLower() == ".jpg" || uzanti.ToLower() == ".png" || uzanti.ToLower() == ".jpeg"))
                    userManager.Photo = ConvertIFormFileToByteArray(companyModel.Logo);
                else
                {
                    ModelState.AddModelError("Logo", "We only accept .jpg, .png and .jpeg formats");
                    await ViewElements();
                    return View();
                }
            }

            if (companyModel.ContractEndDate > DateTime.Now)
                status = true;

            // Create a new Company object and populate its properties
            string lstManagers = "";
            for (int i = 0; i < companyModel.ManagerIds.Count; i++)
            {
                if (i != companyModel.ManagerIds.Count - 1)
                {
                    lstManagers += companyModel.ManagerIds[i] + "_";

                }
                else
                {
                    lstManagers += companyModel.ManagerIds[i];
                }
            }
            //foreach (var item in companyModel.ManagerIds)
            //{
            //    lstManagers += item + "_";
            //}
            Company company = new Company
            {

                OwnerId = lstManagers,
                CompanyName = companyModel.CompanyName,
                CompanyType = companyModel.CompanyType.Replace("_", " "),
                TaxNumber = companyModel.TaxNumber,
                TaxCompany = $"0{companyModel.TaxNumber}16",
                Logo = userManager.Photo,
                PhoneNumber = companyModel.PhoneNumber,
                Address = companyModel.Address,
                Email = $"info@{ConvertTRCharToENChar(companyModel.CompanyName.ToLower().Replace(" ", ""))}.com",
                EmployeeCount = companyModel.EmployeeCount,
                FoundationYear = companyModel.FoundationYear,
                ContractBeginningDate = companyModel.ContractBeginningDate,
                ContractEndDate = companyModel.ContractEndDate,
                Status = status
            };

            var allcompanies = await _companyRepository.GetAllAsync();

            foreach (var item in allcompanies)
            {
                if (item.TaxNumber == companyModel.TaxNumber)
                {
                    ViewData["ErrorMessagee"] = "There is a company registered under the same tax number.";
                    await ViewElements();
                    return View();
                }
            }

            // Save the new company to the database
            await _companyRepository.AddAsync(company);

            // Optionally, you can set a success message in TempData
            TempData["Message"] = "Company created successfully.";

            await ViewElements();
            return View();
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
                Console.WriteLine("Error during converting the file: " + ex.Message);
                return null;
            }
        }

        public string ConvertTRCharToENChar(string text)
        {
            return String.Join("", text.Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
    }
}
