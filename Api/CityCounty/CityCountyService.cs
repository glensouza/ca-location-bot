namespace CALocationBot.Api.CityCounty;

public class CityCountyService : IDisposable
{
    public CityCountyService()
    {
        this.AllCityCounties = JsonConvert.DeserializeObject<List<CityCountyModel>>(File.ReadAllText("CityCounty\\cityCounties.json"));
    }

    public List<CityCountyModel> AllCityCounties { get; set; }

    private void ReleaseUnmanagedResources()
    {
        // release unmanaged resources
        this.AllCityCounties = null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~CityCountyService()
    {
        ReleaseUnmanagedResources();
    }
}
