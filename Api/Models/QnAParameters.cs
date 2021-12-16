namespace CALocationBot.Api.Models;

public class QnAParameters
{
    [JsonProperty("top")]
    public long Top { get; set; }

    [JsonProperty("question")]
    public string Question { get; set; }

    [JsonProperty("includeUnstructuredSources")]
    public bool IncludeUnstructuredSources { get; set; }

    [JsonProperty("confidenceScoreThreshold")]
    public string ConfidenceScoreThreshold { get; set; }

    [JsonProperty("answerSpanRequest")]
    public QnAParameterAnswerSpanRequest AnswerSpanRequest { get; set; }
}