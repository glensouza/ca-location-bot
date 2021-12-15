﻿namespace CALocationBot.Api;

public static class GetIpAddressInfo
{
    [FunctionName("GetIpAddressInfo")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function \"GetIpAddressInfo\" received a request.");

        string clientIpAddress = req.Query["ipAddress"];
        if (string.IsNullOrEmpty(clientIpAddress))
        {
            clientIpAddress = await new StreamReader(req.Body).ReadToEndAsync();
        }

        log.LogInformation(string.IsNullOrEmpty(clientIpAddress)
            ? "No IP Address passed in from client."
            : $"Received IP Address from client: {clientIpAddress}.");

        string remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();
        log.LogInformation(string.IsNullOrEmpty(remoteIpAddress)
            ? "No remote IP Address found for request."
            : $"Found remote IP Address from request: {remoteIpAddress}.");

#if DEBUG
        log.LogInformation("Running in debug mode.");
        if (remoteIpAddress == "127.0.0.1")
        {
            if (string.IsNullOrEmpty(clientIpAddress))
            {
                using HttpClient client = new();
                HttpResponseMessage resp = await client.GetAsync("https://api.ipify.org/");
                remoteIpAddress = await resp.Content.ReadAsStringAsync();
                log.LogInformation($"Local request is running as IP Address {remoteIpAddress}.");
            }
            else
            {
                remoteIpAddress = clientIpAddress;
            }
        }
#else
        log.LogInformation($"Remote request identified as coming from IP Address {remoteIpAddress}.");
#endif

        List<ViewModel> returnValues = new();
        try
        {
            using GeoLite2Service geoLite2 = new (Environment.GetEnvironmentVariable("GeoLiteLicenseKey"), log);
            if (!string.IsNullOrEmpty(clientIpAddress))
            {
                log.LogInformation($"Looking for city information for client IP Address: {clientIpAddress}");
                ViewModel clientIpInfo = geoLite2.GetCityForIpAddress(clientIpAddress);
                if (clientIpInfo != null)
                {
                    log.LogInformation($"Found client city {clientIpInfo.City}");
                    returnValues.Add(clientIpInfo);
                }
            }

            if (clientIpAddress != remoteIpAddress)
            {
                log.LogInformation($"Looking for city information for remote IP Address: {clientIpAddress}");
                ViewModel remoteIpInfo = geoLite2.GetCityForIpAddress(remoteIpAddress);
                if (remoteIpInfo != null)
                {
                    log.LogInformation($"Found client city {remoteIpInfo.City}");
                    returnValues.Add(remoteIpInfo);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Exception in reading from GeoLite database: {ex.Message}.");
            if (!string.IsNullOrEmpty(clientIpAddress))
            {
                returnValues.Add(new ViewModel
                {
                    City = string.Empty,
                    Country = string.Empty,
                    County = string.Empty,
                    IpAddress = clientIpAddress,
                    State = string.Empty
                });
            }

            if (clientIpAddress != remoteIpAddress && !string.IsNullOrEmpty(remoteIpAddress))
            {
                returnValues.Add(new ViewModel
                {
                    City = string.Empty,
                    Country = string.Empty,
                    County = string.Empty,
                    IpAddress = remoteIpAddress,
                    State = string.Empty
                });
            }
        }

        // check if ip address is in california
        if (returnValues.All(returnValue => returnValue.State != "CA"))
        {
            log.LogInformation("Request is not from California.");
            return new OkObjectResult(returnValues);
        }

        using CityCountyService cityCounty = new();
        List<CityCountyModel> counties = cityCounty.AllCityCounties;
        foreach (ViewModel returnValue in returnValues.Where(returnValue => returnValue.State == "CA"))
        {
            // load json file counties (db)
            string returnValueCounty = counties.FirstOrDefault(s => s.City == returnValue.City)?.County;
            returnValue.County = returnValueCounty;
            log.LogInformation($"Identified IP Address {returnValue.IpAddress} as a California City {returnValue.City} in County {returnValue.County}.");
        }

        return new OkObjectResult(returnValues);
    }
}