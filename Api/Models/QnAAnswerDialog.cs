namespace CALocationBot.Api.Models;

public class QnAAnswerDialog
{
    [JsonProperty("isContextOnly")]
    public bool IsContextOnly { get; set; }

    [JsonProperty("prompts")]
    public List<object> Prompts { get; set; }
}