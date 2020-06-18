using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(FoursquareApi.Startup))]

namespace FoursquareApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            builder.Services.AddHttpClient();
            /*
            // builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, (client) =>
            builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, (serviceProvider, client) =>
            {
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

                //serviceProvider.
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(5);

                client.DefaultRequestHeaders.Add("FSQ_CLIENT_ID", clientId);
                client.DefaultRequestHeaders.Add("FSQ_CLIENT_IDFSQ_CLIENT_SECRET", clientSecret);
                client.DefaultRequestHeaders.Add("v", "20190425");
                client.DefaultRequestHeaders.Add("intent", "browse");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            */
        }
    }
}
