namespace CALocationBot.Api.Models;

public class QnAAnswer
{
    [JsonProperty("questions")]
    public List<string> Questions { get; set; }

    [JsonProperty("answer")]
    public string Answer { get; set; }

    [JsonProperty("confidenceScore")]
    public long ConfidenceScore { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("metadata")]
    public QnAAnswerMetadata Metadata { get; set; }

    [JsonProperty("dialog")]
    public QnAAnswerDialog Dialog { get; set; }
}