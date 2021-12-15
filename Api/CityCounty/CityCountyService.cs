namespace CALocationBot.Api.CityCounty;

public class CityCountyService : IDisposable
{
    public CityCountyService(ExecutionContext context)
    {
        string localFile = Path.Combine(context.FunctionAppDirectory, "CityCounty", "cityCounties.json");
        this.AllCityCounties = JsonConvert.DeserializeObject<List<CityCountyModel>>(File.ReadAllText(localFile));
    }

    public List<CityCountyModel> AllCityCounties { get; private set; }

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
