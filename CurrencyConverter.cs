using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;

namespace Sharpvin
{
    internal class ER_API_Obj
    {
        public string result { get; set; }
        public string documentation { get; set; }
        public string terms_of_use { get; set; }
        public string time_last_update_unix { get; set; }
        public string time_last_update_utc { get; set; }
        public string time_next_update_unix { get; set; }
        public string time_next_update_utc { get; set; }
        public string base_code { get; set; }
        public ConversionRate conversion_rates { get; set; }
    }

    internal class ConversionRate
    {
        public double AED { get; set; }
        public double ARS { get; set; }
        public double AUD { get; set; }
        public double BGN { get; set; }
        public double BRL { get; set; }
        public double BSD { get; set; }
        public double CAD { get; set; }
        public double CHF { get; set; }
        public double CLP { get; set; }
        public double CNY { get; set; }
        public double COP { get; set; }
        public double CZK { get; set; }
        public double DKK { get; set; }
        public double DOP { get; set; }
        public double EGP { get; set; }
        public double EUR { get; set; }
        public double FJD { get; set; }
        public double GBP { get; set; }
        public double GTQ { get; set; }
        public double HKD { get; set; }
        public double HRK { get; set; }
        public double HUF { get; set; }
        public double IDR { get; set; }
        public double ILS { get; set; }
        public double INR { get; set; }
        public double ISK { get; set; }
        public double JPY { get; set; }
        public double KRW { get; set; }
        public double KZT { get; set; }
        public double MXN { get; set; }
        public double MYR { get; set; }
        public double NOK { get; set; }
        public double NZD { get; set; }
        public double PAB { get; set; }
        public double PEN { get; set; }
        public double PHP { get; set; }
        public double PKR { get; set; }
        public double PLN { get; set; }
        public double PYG { get; set; }
        public double RON { get; set; }
        public double RUB { get; set; }
        public double SAR { get; set; }
        public double SEK { get; set; }
        public double SGD { get; set; }
        public double THB { get; set; }
        public double TRY { get; set; }
        public double TWD { get; set; }
        public double UAH { get; set; }
        public double USD { get; set; }
        public double UYU { get; set; }
        public double ZAR { get; set; }
    }


    internal class CurrencyConverter
    {
        private readonly HttpClient client;
        private ConversionRate rates;

        public CurrencyConverter()
        {
            client = new HttpClient();
            rates = new ConversionRate();
        }

        public async Task<bool> UpdateRates()
        {
            try
            {
                String url = "https://v6.exchangerate-api.com/v6/413271983b9242697900a781/latest/USD";

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        if (content != null)
                        {
                            String json = content.ReadAsStringAsync().Result;
                            ER_API_Obj obj  = JsonConvert.DeserializeObject<ER_API_Obj>(json);
                            rates = obj.conversion_rates;
                            Console.WriteLine("Rates updated");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("HttpContent is null");
                            return false;
                        }
                            
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public double Convert(double value, String code_from, String code_to)
        {
            code_from = code_from.ToUpper();
            code_to = code_to.ToUpper();

            double rate1, rate2;

            Type typ = rates.GetType();
            PropertyInfo? prop = typ.GetProperty(code_from);
            if (prop != null)
            {
                var r1 = prop.GetValue(rates, null);
                rate1 = (double)r1;
            }
            else
            {
                throw new Exception("Invalid currency code");
            }


            prop = typ.GetProperty(code_to);
            if (prop != null)
            {
                var r2 = prop.GetValue(rates, null);
                rate2 = (double)r2;
            }
            else
            {
                throw new Exception("Invalid currency code");
            }

            double result = value * (rate2 / rate1);
            return Math.Round(result, 2);

        }
    }
}

