namespace CALocationBot.Api.Models;

public class ViewModel : CityCountyModel
{
    [JsonProperty("ipAddress")]
    public string IpAddress { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }
}
