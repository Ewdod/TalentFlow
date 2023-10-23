using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ApplicationCore.Enums;
using Web.Areas.Personnel.Models;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;
using Infrastructure.Services;
using Infrastructure.Models;

namespace Web.Areas.Personnel.Controllers
{
    public class AdvanceRequestController : PersonnelBaseController
    {
        private readonly IRepository<AdvancePaymentRequest> _advanceRepository;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private HttpContext HttpContext => _httpContextAccessor.HttpContext!;
        private string? PersonnelId => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        private ApplicationUser user => _userManager.Users.First(x => x.Id == PersonnelId);


        public AdvanceRequestController(IRepository<AdvancePaymentRequest> advanceRepository, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _advanceRepository = advanceRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> CreateAdvance()
        {
            await ViewElements();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdvance(AdvancePaymentViewModel advanceModel)
        {
            AdvancePaymentRequest advanceRequest = new AdvancePaymentRequest();
            decimal maxBusinessPayment = 100000;
            decimal maxPersonalPayment = user.Salary * 3;
            decimal minPayment = 1000;
            string convertedCurrency = "0";
            decimal decimalConvertedCurrency = advanceModel.Advance;
            string localCurrency = "try";
            CurrencyDTO currencyDTO = new CurrencyDTO();

            if (ModelState.IsValid)
            {
                advanceRequest.PersonnelId = user.Id;
                advanceRequest.Currency = advanceModel.Currency;
                advanceRequest.Advance = Convert.ToString(advanceModel.Advance);
                advanceRequest.Description = advanceModel.Description;
                advanceRequest.Type = advanceModel.AdvanceType;
                advanceRequest.FirstName = user.FirstName;
                advanceRequest.LastName = user.LastName;
                advanceRequest.SecondFirstName = user.SecondLastName;
                advanceRequest.SecondLastName = user.SecondLastName;

                var previousAdvanceRequest = await _advanceRepository.HasPendingAdvangeRequestAsync(user.Id);

                if (previousAdvanceRequest)
                {
                    ViewBag.AdvanceRequestError = "You cannot submit advance payment request while you have pending request";
                    await ViewElements();
                    return View(advanceModel);
                }

                if (advanceModel.Currency.ToLower() != localCurrency)
                {
                    CurrencyConverterAPIService api = new CurrencyConverterAPIService();
                    convertedCurrency = await api.ConvertCurrency(advanceModel.Advance, advanceModel.Currency, localCurrency);
                    if (CultureInfo.CurrentCulture.Name == "tr-TR")
                    {
                        currencyDTO.result = convertedCurrency.Replace(".", ",");
                    }
                    else
                    {
                        currencyDTO.result = convertedCurrency;
                    }
                    decimalConvertedCurrency = Convert.ToDecimal(currencyDTO.result);
                }                
                if (decimalConvertedCurrency < minPayment)
                {
                    await ViewElements();
                    ViewData["Message"] = $"Minimum advance amount determined for the company is {minPayment} TRY.";
                    return View(advanceModel);
                }  
                if (decimalConvertedCurrency > maxPersonalPayment && advanceModel.AdvanceType == AdvancePaymentType.Personal)
                {
                    await ViewElements();
                    ViewData["Message"] = $"You can request personal advance up to 3 salaries({maxPersonalPayment}).";
                    return View(advanceModel);
                }
                else if (decimalConvertedCurrency > maxBusinessPayment && advanceModel.AdvanceType == AdvancePaymentType.Business)
                {
                    await ViewElements();
                    ViewData["Message"] = $"Maximum advance amount determined for the company is {maxBusinessPayment} TRY.";
                    return View(advanceModel);
                }
                else
                {
                    await _advanceRepository.AddAsync(advanceRequest);
                    await ViewElements();
                    ViewData["Message"] = "Your advance request has been sent successfully";
                    return View(advanceModel);
                }
            }
            else
            {
                return View();
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
