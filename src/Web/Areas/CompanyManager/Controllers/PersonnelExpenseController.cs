using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.CompanyManager.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Reflection.Metadata;
using Document = iTextSharp.text.Document;

namespace Web.Areas.CompanyManager.Controllers
{
    public class PersonnelExpenseController : CompanyManagerBaseController
    {
        private readonly IRepository<ExpenseRequest> _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TalentFlowDbContext _db;

        public PersonnelExpenseController(IRepository<ExpenseRequest> repository, UserManager<ApplicationUser> userManager, TalentFlowDbContext db)
        {
            _repository = repository;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> ExpenseRequest()
        {
            await ViewElements();
            var pendingRequests = await _repository.GetAllAsync();
            var pendingExpenseRequests = pendingRequests.Where(r => r.Status == Status.Pending).ToList();
            return View("ExpenseRequest", pendingExpenseRequests);
        }

        public async Task<IActionResult> ListAcceptedExpenseRequests()
        {
            await ViewElements();
            ViewBag.Status = "Accepted";
            var acceptedRequests = await _repository.GetAllAsync();
            var acceptedExpenseRequests = acceptedRequests.Where(r => r.Status == Status.Accepted).ToList();
            return View("ExpenseRequest", acceptedExpenseRequests);
        }

        public async Task<IActionResult> ListRejectedExpenseRequests()
        {
            await ViewElements();
            ViewBag.Status = "Rejected";
            var rejectedRequests = await _repository.GetAllAsync();
            var rejectedExpenseRequests = rejectedRequests.Where(r => r.Status == Status.Rejected).ToList();
            return View("ExpenseRequest", rejectedExpenseRequests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await ViewElements();
            try
            {
                var expenseRequest = await _repository.GetByIdAsync(id);

                if (expenseRequest != null)
                {

                    var userId = expenseRequest.PersonnelId;
                    var user = await _userManager.FindByIdAsync(userId);
                    // Check if the status string is a valid enum value

                    if (Enum.TryParse(status, out Status newStatus))
                    {
                        expenseRequest.Status = newStatus;
                        expenseRequest.ResponseDate = DateTime.Now;
                        await _repository.UpdateAsync(expenseRequest);

                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Invalid status value");
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public IActionResult ShowFile(int expenseRequestId)
        {
            var expenseRequest = _db.ExpenseRequests.Find(expenseRequestId);

            if (expenseRequest != null && expenseRequest.DocumentPath != null)
            {
                string fileExtension = null;

                if (expenseRequest.DocumentPath.Length >= 4)
                {
                    if (expenseRequest.DocumentPath[0] == 0xFF && expenseRequest.DocumentPath[1] == 0xD8 && expenseRequest.DocumentPath[2] == 0xFF)
                    {
                        fileExtension = ".jpeg";
                    }
                    else if (expenseRequest.DocumentPath[0] == 0x89 && expenseRequest.DocumentPath[1] == 0x50 && expenseRequest.DocumentPath[2] == 0x4E && expenseRequest.DocumentPath[3] == 0x47)
                    {
                        fileExtension = ".png";
                    }
                    else if (expenseRequest.DocumentPath[0] == 0x25 && expenseRequest.DocumentPath[1] == 0x50 && expenseRequest.DocumentPath[2] == 0x44 && expenseRequest.DocumentPath[3] == 0x46)
                    {
                        fileExtension = ".pdf";
                    }
                    else if (expenseRequest.DocumentPath[0] == 0xFF && expenseRequest.DocumentPath[1] == 0xD8 && expenseRequest.DocumentPath[2] == 0xFF && expenseRequest.DocumentPath[3] == 0xE0)
                    {
                        fileExtension = ".jpg";
                    }
                }

                if (fileExtension != null)
                {

                    string contentType = "application/octet-stream";
                    switch (fileExtension)
                    {
                        case ".pdf":
                            contentType = "application/pdf";
                            break;
                        case ".jpeg":
                        case ".jpg":
                            contentType = "image/jpeg";
                            break;
                        case ".png":
                            contentType = "image/png";
                            break;
                    }

                    return File(expenseRequest.DocumentPath, contentType);
                }
            }
            return NotFound();
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
