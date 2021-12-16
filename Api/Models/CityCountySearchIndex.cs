namespace CALocationBot.Api.Models;

public class CityCountySearchIndex
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string CityId { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string City { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string County { get; set; }
}
