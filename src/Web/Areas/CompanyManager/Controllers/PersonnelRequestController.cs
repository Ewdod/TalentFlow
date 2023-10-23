using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using Web.Areas.CompanyManager.Models;
using Document = iTextSharp.text.Document;

namespace Web.Areas.CompanyManager.Controllers
{
    public class PersonnelRequestController : CompanyManagerBaseController
    {
        private readonly IRepository<LeaveRequest> _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TalentFlowDbContext _db;

        public PersonnelRequestController(IRepository<LeaveRequest> repository, UserManager<ApplicationUser> userManager, TalentFlowDbContext db)
        {
            _repository = repository;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> LeaveRequest()
        {
            await ViewElements();
            var pendingRequests = await _repository.GetAllAsync();
            var pendingLeaveRequests = pendingRequests.Where(r => r.Status == Status.Pending).ToList();
            return View("LeaveRequest", pendingLeaveRequests);
        }

        public async Task<IActionResult> ListAcceptedLeaveRequests()
        {
            await ViewElements();
            ViewBag.Status = "Accepted";
            var acceptedRequests = await _repository.GetAllAsync();
            var acceptedLeaveRequests = acceptedRequests.Where(r => r.Status == Status.Accepted).ToList();
            return View("LeaveRequest", acceptedLeaveRequests);
        }

        public async Task<IActionResult> ListRejectedLeaveRequests()
        {
            await ViewElements();
            ViewBag.Status = "Rejected";
            var rejectedRequests = await _repository.GetAllAsync();
            var rejectedLeaveRequests = rejectedRequests.Where(r => r.Status == Status.Rejected).ToList();
            return View("LeaveRequest", rejectedLeaveRequests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await ViewElements();
            try
            {
                var leaveRequest = await _repository.GetByIdAsync(id);

                if (leaveRequest != null)
                {
                    var userId = leaveRequest.PersonnelId;
                    var user = await _userManager.FindByIdAsync(userId);                   

                    if (Enum.TryParse(status, out Status newStatus))
                    {
                        if (newStatus == Status.Rejected)
                        {
                            int sundayCount = 0;
                            for (DateTime i = leaveRequest.StartDate.Date; i <= leaveRequest.EndDate.Date; i = i.AddDays(1))
                            {
                                if (i.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    sundayCount++;
                                }
                            }
                            user.AnnualLeaveAllowance = user.AnnualLeaveAllowance + leaveRequest.NumberOfDays - sundayCount;
                            await _userManager.UpdateAsync(user);
                        }
                      
                        leaveRequest.Status = newStatus;
                        leaveRequest.ResponseDate = DateTime.Now;
                        await _repository.UpdateAsync(leaveRequest); 

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

        public IActionResult ShowFile(int leaveRequestId)
        {
            var leaveRequest = _db.LeaveRequests.Find(leaveRequestId);

            if (leaveRequest != null && leaveRequest.DocumentPath != null)
            {
                string fileExtension = null;
                
                if (leaveRequest.DocumentPath.Length >= 4)
                {
                    if (leaveRequest.DocumentPath[0] == 0xFF && leaveRequest.DocumentPath[1] == 0xD8 && leaveRequest.DocumentPath[2] == 0xFF)
                    {
                        fileExtension = ".jpeg"; 
                    }
                    else if (leaveRequest.DocumentPath[0] == 0x89 && leaveRequest.DocumentPath[1] == 0x50 && leaveRequest.DocumentPath[2] == 0x4E && leaveRequest.DocumentPath[3] == 0x47)
                    {
                        fileExtension = ".png"; 
                    }
                    else if (leaveRequest.DocumentPath[0] == 0x25 && leaveRequest.DocumentPath[1] == 0x50 && leaveRequest.DocumentPath[2] == 0x44 && leaveRequest.DocumentPath[3] == 0x46)
                    {
                        fileExtension = ".pdf";
                    }
                    else if (leaveRequest.DocumentPath[0] == 0xFF && leaveRequest.DocumentPath[1] == 0xD8 && leaveRequest.DocumentPath[2] == 0xFF && leaveRequest.DocumentPath[3] == 0xE0)
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
                    return File(leaveRequest.DocumentPath, contentType);
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

        static string BytesToStringConverter(byte[] byteArray)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
