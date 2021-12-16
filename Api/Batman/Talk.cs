namespace CALocationBot.Api.Batman;

public class Talk
{
    private readonly HttpClient httpClient;
    private readonly string url;

    public Talk(HttpClient httpClient, string qnaKnowledgebaseId, string qnaEndpointKey, string qnaEndpointHostName)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", qnaEndpointKey);
        this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        this.url = $"{qnaEndpointHostName}language/:query-knowledgebases?projectName={qnaKnowledgebaseId}&api-version=2021-10-01&deploymentName=production";
    }

    public async Task<string> AskQuestion(string question)
    {
        QnAParameters parameters = new()
        {
            Top = 1,
            Question = question,
            IncludeUnstructuredSources = true,
            ConfidenceScoreThreshold = ".75",
            AnswerSpanRequest = new QnAParameterAnswerSpanRequest
            {
                Enable = true,
                TopAnswersWithSpan = 1,
                ConfidenceScoreThreshold = ".75"
            }
        };
        string json = JsonConvert.SerializeObject(parameters);
        StringContent data = new(json, Encoding.UTF8, "application/json");
        // The actual call to the QnA Maker service.
        HttpResponseMessage response = await this.httpClient.PostAsync(this.url, data);
        string result = await response.Content.ReadAsStringAsync();
        QnAAnswerToQuestion answers = JsonConvert.DeserializeObject<QnAAnswerToQuestion>(result);
        return answers.Answers.FirstOrDefault()?.Answer;
    }
}
