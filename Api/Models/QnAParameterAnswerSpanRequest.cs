namespace CALocationBot.Api.Models;

public class QnAParameterAnswerSpanRequest
{
    [JsonProperty("enable")]
    public bool Enable { get; set; }

    [JsonProperty("topAnswersWithSpan")]
    public long TopAnswersWithSpan { get; set; }

    [JsonProperty("confidenceScoreThreshold")]
    public string ConfidenceScoreThreshold { get; set; }
}
