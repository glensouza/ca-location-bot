namespace CALocationBot.Api.GeoLite2_City;

public class GeoLite2Service : IDisposable
{
    private const string GeoDirectory = "GeoLite2-City";
    private const string GeoFileName = $"{GeoDirectory}\\GeoLite2-City.mmdb";
    private const string GeoTarFileName = $"{GeoFileName}.tar.gz";
    private readonly DatabaseReader reader;

    public GeoLite2Service(string licenseKey)
    {
        // check if database file exists and is older than a day
        if (!File.Exists(GeoFileName) || File.GetLastWriteTimeUtc(GeoFileName) < DateTime.UtcNow.AddDays(-1))
        {
            if (File.Exists(GeoTarFileName))
            {
                File.Delete(GeoTarFileName);
            }

            if (File.Exists(GeoFileName))
            {
                File.Delete(GeoFileName);
            }

            Uri uri = new($"https://download.maxmind.com/app/geoip_download?edition_id=GeoLite2-City&license_key={licenseKey}&suffix=tar.gz");
            using HttpClient client = new();
            byte[] fileBytes = client.GetByteArrayAsync(uri).Result;
            File.WriteAllBytes($"{GeoFileName}.tar.gz", fileBytes);

            using Stream inStream = File.OpenRead($"{GeoFileName}.tar.gz");
            using Stream gzipStream = new GZipInputStream(inStream);
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(".");
            tarArchive.Close();
            gzipStream.Close();
            inStream.Close();
            File.Delete(GeoTarFileName);

            string directory = Directory.GetDirectories(".").FirstOrDefault(s => s.Contains("GeoLite2-City_"));
            if (string.IsNullOrEmpty(directory))
            {
                throw new Exception("Couldn't download the Geo database file properly.");
            }

            try
            {
                Directory.Delete(GeoDirectory, true);
            }
            catch (IOException)
            {
                Directory.Delete(GeoDirectory, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(GeoDirectory, true);
            }
            Directory.Move(directory, GeoDirectory);
        }

        /*
            For database usage see the API documentation:
            https://maxmind.github.io/GeoIP2-dotnet/
            https://github.com/maxmind/GeoIP2-dotnet#city-database
        */
        this.reader = new DatabaseReader(GeoFileName);
    }

    public ViewModel GetCityForIpAddress(string ipAddress)
    {
        CityResponse city = null;
        try
        {
            city = reader.City(ipAddress);
        }
        catch
        {
            // ignored
        }

        ViewModel returnValue = new()
        {
            IpAddress = ipAddress,
            Country = city?.Country.IsoCode, // 'US'
            State = city?.MostSpecificSubdivision.IsoCode, // 'CA'
            City = city?.City.Name // 'Corona'
        };
        return returnValue;
    }

    private void ReleaseUnmanagedResources()
    {
        this.reader?.Dispose();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~GeoLite2Service()
    {
        ReleaseUnmanagedResources();
    }
}
