namespace CALocationBot.Api.Models;

public class CityCountyModel
{
    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("county")]
    public string County { get; set; }
}
