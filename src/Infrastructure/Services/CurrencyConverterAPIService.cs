using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using Infrastructure.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class CurrencyConverterAPIService
    {
        private readonly string _apiKey = "IFXKFmS52a04sdQcTfNdAH0lZ5AroObA";
        private readonly string baseAddress = "https://api.apilayer.com/fixer/";
        private readonly HttpClient _httpClient;

        public CurrencyConverterAPIService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        }

        public async Task<string> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            CurrencyDTO currencyDTO = new CurrencyDTO();

            string endpoint = $"convert?to={toCurrency.ToLower()}&from={fromCurrency.ToLower()}&amount={amount}";
           
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            

            if (response.IsSuccessStatusCode)
            {                
                var jsonresponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonresponse);
                var root = jsonDocument.RootElement;

                // "result" alanını çıkar
                if (root.TryGetProperty("result", out var resultProperty))
                {
                    return resultProperty.ToString();
                }
            }
            return null;
        }
    }
}
