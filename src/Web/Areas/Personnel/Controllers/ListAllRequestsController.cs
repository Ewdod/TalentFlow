using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Web.Areas.Personnel.Models;

namespace Web.Areas.Personnel.Controllers
{
    public class ListAllRequestsController : PersonnelBaseController
    {
        private readonly IRepository<LeaveRequest> _leaveRepository;
        private readonly IRepository<AdvancePaymentRequest> _advanceRepository;
        private readonly IRepository<ExpenseRequest> _expenseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListAllRequestsController(
            IRepository<LeaveRequest> leaveRepository,
            IRepository<AdvancePaymentRequest> advanceRepository,
            IRepository<ExpenseRequest> expenseRepository,
            UserManager<ApplicationUser> userManager)
        {
            _leaveRepository = leaveRepository;
            _advanceRepository = advanceRepository;
            _expenseRepository = expenseRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> ListLeaveRequests()
        {
            await ViewElements();
            var currentUser = await _userManager.GetUserAsync(User);
            var personnelId = currentUser.Id;
            var leaveRequests = await _leaveRepository.GetAllAsync();
            var userLeaveRequests = leaveRequests.Where(r => r.PersonnelId == personnelId).ToList();

            var viewModel = new ListAllRequestsViewModel
            {
                LeaveRequests = userLeaveRequests
            };

            return View("ListLeaveRequests", viewModel); // Do not include the folder structure
        }

        public async Task<IActionResult> ListAdvanceRequests()
        {
            await ViewElements();
            var currentUser = await _userManager.GetUserAsync(User);
            var personnelId = currentUser.Id;
            var advanceRequests = await _advanceRepository.GetAllAsync();
            var userAdvanceRequests = advanceRequests.Where(r => r.PersonnelId == personnelId).ToList();

            var viewModel = new ListAllRequestsViewModel
            {
                AdvanceRequests = userAdvanceRequests
            };

            return View("ListAdvanceRequests", viewModel);
        }

        public async Task<IActionResult> ListExpenseRequests()
        {
            await ViewElements();
            var currentUser = await _userManager.GetUserAsync(User);
            var personnelId = currentUser.Id;
            var expenseRequests = await _expenseRepository.GetAllAsync();
            var userExpenseRequests = expenseRequests.Where(r => r.PersonnelId == personnelId).ToList();

            var viewModel = new ListAllRequestsViewModel
            {
                ExpenseRequests = userExpenseRequests
            };

            return View("ListExpenseRequests", viewModel);
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
