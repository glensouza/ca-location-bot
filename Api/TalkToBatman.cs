using CALocationBot.Api.Batman;

namespace CALocationBot.Api;

public class TalkToBatman
{
    private readonly HttpClient httpClient;

    public TalkToBatman(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    [FunctionName("TalkToBatman")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string question = req.Query["question"];
        string qnaKnowledgebaseId = Environment.GetEnvironmentVariable("QnAKnowledgebaseId");
        string qnaEndpointKey = Environment.GetEnvironmentVariable("QnAEndpointKey");
        string qnaEndpointHostName = Environment.GetEnvironmentVariable("QnAEndpointHostName");
        Talk batman = new(this.httpClient, qnaKnowledgebaseId, qnaEndpointKey, qnaEndpointHostName);
        string answer = await batman.AskQuestion(question);
        return new OkObjectResult(answer);
    }
}
