using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Globalization;
using Web.Areas.Personnel.Models;

namespace Web.Areas.Personnel.Controllers
{
    public class ExpenseRequestController : PersonnelBaseController
    {
        private readonly IRepository<ExpenseRequest> _repository;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public ExpenseRequestController(IRepository<ExpenseRequest> repository, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }


        public async Task<IActionResult> CreateExpense()
        {
            await ViewElements();
            var userName = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var expenseList = Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>();

            ViewBag.ExpenseList = expenseList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExpense(ExpenseRequestModel model)
        {
            var expenseList = Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>();
            ViewBag.ExpenseList = expenseList;
            string convertedCurrency = "0";
            decimal decimalConvertedCurrency = 0;
            string localCurrency = "try";
            CurrencyDTO currencyDTO = new CurrencyDTO();

            if (ModelState.IsValid)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);

                // Kullanıcının daha önceden aynı türde bir istek yapmış olup olmadığını kontrol et
                var existingExpenseRequest = await _repository.GetExpenseRequestByTypeAndUserId(user.Id, model.ExpenseType);

                if (existingExpenseRequest != null)
                {
                    await ViewElements();
                    ViewBag.ExpenseAlreadyCreated = "You have already submitted an expense request of this type.";
                    return View(model);
                }

                var expenseRequest = new ExpenseRequest
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PersonnelId = user.Id,
                    Type = model.ExpenseType,
                    Expense = Convert.ToString(model.Expense),
                    Currency = model.Currency,
                    DocumentPath = ConvertIFormFileToByteArray(model.Document)
                };

                ExpenseTypeInfo expenseTypeInfo = ExpenseTypeData.ExpenseTypes.FirstOrDefault(x => x.Type == expenseRequest.Type);

                if (model.Currency.ToLower() != localCurrency)
                {
                    CurrencyConverterAPIService api = new CurrencyConverterAPIService();
                    convertedCurrency = await api.ConvertCurrency(model.Expense, model.Currency, localCurrency);
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

                if (expenseTypeInfo != null)
                { 
                    if(decimalConvertedCurrency < expenseTypeInfo.MinExpense)
                    {
                        await ViewElements();
                        TempData["Error"] = $"Minimum expense request amount for {expenseTypeInfo.Type} is {expenseTypeInfo.MinExpense} TRY";
                        return View(model);
                    }
                    else if (decimalConvertedCurrency > expenseTypeInfo.MaxExpense)
                    {
                        await ViewElements();
                        TempData["Error"] = $"Maximum expense request amount for {expenseTypeInfo.Type} is {expenseTypeInfo.MaxExpense} TRY";
                        return View(model);
                    }

                    await _repository.AddAsync(expenseRequest);

                    TempData["Message"] = "Expense request has been submitted successfully.";
                    await ViewElements();
                    return View(model);
                }

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
