using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Personnel.Models;
using ApplicationCore.Enums;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using LeaveRequest = ApplicationCore.Entities.LeaveRequest;
using System.Reflection.Metadata;

namespace Web.Areas.Personnel.Controllers
{
    public class LeaveRequestController : PersonnelBaseController
    {
        private readonly IRepository<LeaveRequest> _leaveRepository;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly TalentFlowDbContext _db;

        public LeaveRequestController(IRepository<LeaveRequest> leaveRepository, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, TalentFlowDbContext db)
        {
            _leaveRepository = leaveRepository;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Create()
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            List<LeaveTypeInfo> leaveTypeInfos = new List<LeaveTypeInfo>();

            var previousLeaveRequest = await _leaveRepository.HasPendingLeaveRequestAsync(user.Id);

            if (previousLeaveRequest)
            {
                ViewBag.LeaveRequestError = "You cannot submit leave request while you have pending request";
                leaveTypeInfos.AddRange(LeaveTypeData.LeaveTypes);
                if (user.Gender == Gender.Male)
                {
                    var maleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Male || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                    ViewBag.LeaveList = maleLeaveList;
                }
                else
                {
                    var femaleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Female || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                    ViewBag.LeaveList = femaleLeaveList;
                }
                ViewData["AnnualLeave"] = user.AnnualLeaveAllowance;
                return View();
            }
            else
            {
                leaveTypeInfos.AddRange(LeaveTypeData.LeaveTypes);
                if (user.Gender == Gender.Male)
                {
                    var maleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Male || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                    ViewBag.LeaveList = maleLeaveList;
                }
                else
                {
                    var femaleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Female || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                    ViewBag.LeaveList = femaleLeaveList;
                }
                ViewData["AnnualLeave"] = user.AnnualLeaveAllowance;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestModel model)
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            ViewData["AnnualLeave"] = user.AnnualLeaveAllowance;
            List<LeaveTypeInfo> leaveTypeInfos = new List<LeaveTypeInfo>();
            leaveTypeInfos.AddRange(LeaveTypeData.LeaveTypes);

            if (user.Gender == Gender.Male)
            {
                var maleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Male || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                ViewBag.LeaveList = maleLeaveList;
            }
            else
            {
                var femaleLeaveList = leaveTypeInfos.Where(x => x.Gender == Gender.Female || x.Gender == Gender.None).Select(x => x.Type).Distinct().ToList();
                ViewBag.LeaveList = femaleLeaveList;
            }
            if (ModelState.IsValid)
            {       
                var leaveRequest = new LeaveRequest
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PersonnelId = user.Id,
                    Type = model.LeaveType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DocumentPath = ConvertIFormFileToByteArray(model.Document)
                };

                TimeSpan workingYears = DateTime.Now.Date - user.StartDate.Date;
                LeaveTypeInfo leaveTypeInfo;
                if (model.LeaveType == LeaveType.Annual_Leave)
                {
                    leaveTypeInfo = LeaveTypeData.LeaveTypes.FirstOrDefault(x => x.Type == leaveRequest.Type && x.MinYearsOfWork <= workingYears.TotalDays / 365 && x.MaxYearsOfWork >= workingYears.TotalDays / 365 && (workingYears.TotalDays / 365 <= x.MaxYearsOfWork && workingYears.TotalDays / 365 > x.MinYearsOfWork));
                }
                else
                {
                    leaveTypeInfo = LeaveTypeData.LeaveTypes.FirstOrDefault(x => x.Type == leaveRequest.Type);
                }
                var previousLeaveRequest = await _leaveRepository.HasPendingLeaveRequestAsync(user.Id);
                if (previousLeaveRequest)
                {
                    ViewBag.LeaveRequestError = "You cannot submit leave request while you have pending request";
                    return View();
                }
                if (leaveTypeInfo != null && leaveTypeInfo.Type == LeaveType.Annual_Leave)
                {
                    int sundayCount = 0;
                    for (DateTime i = leaveRequest.StartDate.Date; i <= leaveRequest.EndDate.Date; i = i.AddDays(1))
                    {
                        if (i.DayOfWeek == DayOfWeek.Sunday)
                        {
                            sundayCount++;
                        }
                    }
                    var requestedLeaveDays = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).TotalDays;
                    double totaldays = 0;
                    if (sundayCount > 0)
                    {
                        totaldays = requestedLeaveDays - sundayCount;
                    }

                    if (totaldays > user.AnnualLeaveAllowance || user.AnnualLeaveAllowance <= 0)
                    {
                        ModelState.AddModelError("AnnualLeaveAllowance", "You cannot request more than your allowance.");
                        ViewBag.AnnualLeaveAllowanceError = "You cannot request more than your allowance.";
                        return View(model);
                    }
                    else
                    {
                        user.AnnualLeaveAllowance = user.AnnualLeaveAllowance - (int)totaldays;
                        await _userManager.UpdateAsync(user);
                        await _leaveRepository.AddAsync(leaveRequest);
                        ViewData["AnnualLeave"] = user.AnnualLeaveAllowance;
                        TempData["Message"] = "Leave request has been submitted successfully.";
                        return View(model);
                    }
                }

                if (leaveTypeInfo != null && (leaveTypeInfo.Type == LeaveType.Maternity_Leave || leaveTypeInfo.Type == LeaveType.Paternity_Leave || leaveTypeInfo.Type == LeaveType.Bereavement_Leave || leaveTypeInfo.Type == LeaveType.Marriage_Leave || leaveTypeInfo.Type == LeaveType.Companion_Leave))
                {
                    if (model.Document == null)
                    {
                        ModelState.AddModelError("Document", "Please submit the necessary document.");
                        return View(model);
                    }
                    else
                    {
                        string filePath = model.Document.FileName;
                        string extension = Path.GetExtension(filePath);
                        if (extension != null && (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".pdf" || extension.ToLower() == ".doc" || extension.ToLower() == ".docx"))
                        {
                            await _leaveRepository.AddAsync(leaveRequest);
                            TempData["Message"] = "Leave request has been submitted successfully.";
                            await ViewElements();
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("Document", "Please choose a valid file format (.pdf, .docx, .docx, .jpg, .jpeg, .png).");
                            return View(model);
                        }
                    }
                }

                if (leaveTypeInfo != null && leaveTypeInfo.Type == LeaveType.Sick_Leave && model.Document == null)
                {
                    var requestedLeaveDays = (leaveRequest.EndDate.Date - leaveRequest.StartDate.Date).Days;
                    if (requestedLeaveDays <= 2)
                    {
                        await _leaveRepository.AddAsync(leaveRequest);
                        TempData["Message"] = "Leave request has been submitted successfully.";
                        return View(model);
                    }
                    else
                    {
                        if (model.Document == null)
                        {
                            ModelState.AddModelError("Document", "Please submit the necessary document.");
                            return View(model);
                        }
                        else
                        {
                            string filePath = model.Document.FileName;
                            string extension = Path.GetExtension(filePath);
                            if (extension != null && (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".pdf" || extension.ToLower() == ".doc" || extension.ToLower() == ".docx"))
                            {
                                await _leaveRepository.AddAsync(leaveRequest);
                                TempData["Message"] = "Leave request has been submitted successfully.";
                                await ViewElements();
                                return View(model);
                            }
                            else
                            {
                                ModelState.AddModelError("Document", "Please choose a valid file format (.pdf, .docx, .docx, .jpg, .jpeg, .png).");
                                return View(model);
                            }
                        }
                    }
                }
                if (leaveTypeInfo != null && leaveTypeInfo.Type == LeaveType.Free_Leave)
                {
                    await _leaveRepository.AddAsync(leaveRequest);
                    TempData["Message"] = "Leave request has been submitted successfully.";
                    return View(model);
                }
                await _leaveRepository.AddAsync(leaveRequest);
                TempData["Message"] = "Leave request has been submitted successfully.";
                return View(model);
                //}                                
            }
            return View(model); 
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
            if (file == null)
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
