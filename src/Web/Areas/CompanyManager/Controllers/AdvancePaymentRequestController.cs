using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.CompanyManager.Controllers
{
    public class AdvancePaymentRequestController : CompanyManagerBaseController
    {
        private readonly IRepository<AdvancePaymentRequest> _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdvancePaymentRequestController(IRepository<AdvancePaymentRequest> repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<IActionResult> AdvanceRequests()
        {
            await ViewElements();
            var pendingRequests = await _repository.GetAllAsync();
            var pendingAdvanceRequests = pendingRequests.Where(r => r.Status == Status.Pending).ToList();
            return View("AdvanceRequests", pendingAdvanceRequests);
        }

        public async Task<IActionResult> ListAcceptedAdvanceRequests()
        {
            await ViewElements();
            ViewBag.Status = "Accepted";
            var acceptedRequests = await _repository.GetAllAsync();
            var acceptedAdvanceRequests = acceptedRequests.Where(r => r.Status == Status.Accepted).ToList();
            return View("AdvanceRequests", acceptedAdvanceRequests);
        }

        public async Task<IActionResult> ListRejectedAdvanceRequests()
        {
            await ViewElements();
            ViewBag.Status = "Rejected";
            var rejectedRequests = await _repository.GetAllAsync();
            var rejectedAdvanceRequests = rejectedRequests.Where(r => r.Status == Status.Rejected).ToList();
            return View("AdvanceRequests", rejectedAdvanceRequests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await ViewElements();
            try
            {
                var advanceRequest = await _repository.GetByIdAsync(id);

                if (advanceRequest != null)
                {
                    if (Enum.TryParse(status, out Status newStatus))
                    {
                        advanceRequest.Status = newStatus;
                        advanceRequest.ResponseDate = DateTime.Now;
                        await _repository.UpdateAsync(advanceRequest);

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
