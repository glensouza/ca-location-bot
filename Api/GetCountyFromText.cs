namespace CALocationBot.Api;

public static class GetCountyFromText
{
    [FunctionName("GetCountyFromText")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ExecutionContext context,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string text = req.Query["text"];
        string serviceName = Environment.GetEnvironmentVariable("SearchServiceName");
        string indexName = Environment.GetEnvironmentVariable("SearchIndexName");
        string apiKey = Environment.GetEnvironmentVariable("SearchApiKey");
        SearchService search = new(serviceName, indexName, apiKey, context, log);
        CityCountyModel searchResult = search.Search(text);
        if (searchResult == null || string.IsNullOrEmpty(searchResult.County))
        {
            return new OkResult();
        }

        return new OkObjectResult(new CityCountyViewModel
        {
            County = searchResult.County,
            City = searchResult.City,
            State = "CA"
        });
    }
}
