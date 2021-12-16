namespace CALocationBot.Api.Models
{
    public class QnAAnswerToQuestion
    {
        [JsonProperty("answers")]
        public List<QnAAnswer> Answers { get; set; }
    }
}
