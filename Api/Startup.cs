[assembly: FunctionsStartup(typeof(CALocationBot.Api.Startup))]

namespace CALocationBot.Api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
    }
}
