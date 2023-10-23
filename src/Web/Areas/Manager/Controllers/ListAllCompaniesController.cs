using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Entities; // Eğer gerekirse bu isim alanını ekleyin
using ApplicationCore.Interfaces;
using Web.Areas.Manager.Models; // Eğer gerekirse bu isim alanını ekleyin
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Web.Areas.Personnel.Models;


namespace Web.Areas.Manager.Controllers
{
    public class ListAllCompaniesController : ManagerBaseController
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListAllCompaniesController(IRepository<Company> companyRepository, UserManager<ApplicationUser> userManager)
        {
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> ListAllCompanies(bool showActive = true)
        {
            await ViewElements();

            List<Company> companies;

            if (showActive)
            {
                companies = (await _companyRepository.GetAllAsync()).Where(s => s.Status == true).ToList();
            }
            else
            {
                companies = (await _companyRepository.GetAllAsync()).Where(s => s.Status == false).ToList();
            }

            // Logo resimlerini al
            var logos = companies.Select(c => Convert.ToBase64String(c.Logo)).ToList();
            ViewBag.Logos = logos;

            return View(companies);
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
