using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Api
{
    public static class WeatherForecastFunction
    {
        private static string GetSummary(int temp)
        {
            string summary = temp switch
            {
                >= 32 => "Hot",
                <= 16 and > 0 => "Cold",
                <= 0 => "Freezing!",
                _ => "Mild"
            };

            return summary;
        }

        [FunctionName("WeatherForecast")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            Random randomNumber = new();

            int[] result = Enumerable.Range(1, 5).ToArray();

            return new OkObjectResult(result);
        }
    }
}
