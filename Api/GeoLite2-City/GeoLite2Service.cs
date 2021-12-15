﻿using System.IO.Compression;

namespace CALocationBot.Api.GeoLite2_City;

public class GeoLite2Service : IDisposable
{
    private const string GeoDirectory = "GeoLite2-City";
    private const string GeoFileName = $"{GeoDirectory}\\{GeoDirectory}.mmdb";
    private readonly string geoTarFileName = $"{Path.GetTempPath()}\\{GeoDirectory}.tar.gz";
    private readonly DatabaseReader reader;
    private readonly ILogger logger;

    public GeoLite2Service(string licenseKey, ILogger logger)
    {
        this.logger = logger;

        // check if database file exists and is older than a day
        if (!File.Exists(GeoFileName) || File.GetLastWriteTimeUtc(GeoFileName) < DateTime.UtcNow.AddDays(-1))
        {
            if (File.Exists(this.geoTarFileName))
            {
                logger.LogInformation($"Deleting existing tar file {this.geoTarFileName}");
                File.Delete(this.geoTarFileName);
            }

            if (File.Exists(GeoFileName))
            {
                logger.LogInformation($"Deleting existing geo file {GeoFileName}");
                File.Delete(GeoFileName);
            }

            Uri uri = new($"https://download.maxmind.com/app/geoip_download?edition_id=GeoLite2-City&license_key={licenseKey}&suffix=tar.gz");
            using HttpClient client = new();
            logger.LogInformation("Downloading fresh geoLite file");
            byte[] fileBytes = client.GetByteArrayAsync(uri).Result;
            logger.LogInformation("Downloaded fresh geoLite file");
            using MemoryStream fileStream = new(fileBytes);
            logger.LogInformation("Attempting to decompress geoLite file 1/2");
            using Stream gzipStream = new GZipInputStream(fileStream);
            logger.LogInformation("Attempting to decompress geoLite file 2/2");
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            logger.LogInformation("Extracting contents to directory");
            tarArchive.ExtractContents(".");
            tarArchive.Close();
            gzipStream.Close();
            fileStream.Close();
            logger.LogInformation($"Deleting tar file {this.geoTarFileName}");
            File.Delete(this.geoTarFileName);

            string directory = Directory.GetDirectories(".").FirstOrDefault(s => s.Contains("GeoLite2-City_"));
            if (string.IsNullOrEmpty(directory))
            {
                logger.LogError("Couldn't download the Geo database file properly.");
                throw new Exception("Couldn't download the Geo database file properly.");
            }

            try
            {
                logger.LogInformation($"Deleting directory {GeoDirectory}");
                Directory.Delete(GeoDirectory, true);
            }
            catch (IOException)
            {
                logger.LogError($"Deleting directory {GeoDirectory} again in IOException.");
                Directory.Delete(GeoDirectory, true);
            }
            catch (UnauthorizedAccessException)
            {
                logger.LogError($"Deleting directory {GeoDirectory} again in UnauthorizedAccessException.");
                Directory.Delete(GeoDirectory, true);
            }

            logger.LogInformation($"Renaming directory {directory} to {GeoDirectory}.");
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
            logger.LogInformation($"Looking for city from IP Address {ipAddress}");
            city = reader.City(ipAddress);
            logger.LogInformation($"Found city {city.City} from IP Address {ipAddress}");
        }
        catch
        {
            logger.LogError($"Error reading city from IP Address {ipAddress}");
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
        logger.LogInformation("Releasing Unmanaged Resources");
        this.reader?.Dispose();
    }

    public void Dispose()
    {
        logger.LogInformation("Disposing");
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~GeoLite2Service()
    {
        logger.LogInformation("Auto Dispose");
        ReleaseUnmanagedResources();
    }
}
