using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using FoursquareApi.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FoursquareApi
{
    public class FoursquarePlacesController
    {
        /*
        readonly IHttpClientFactory _httpClientFactory;
       
        public FoursquarePlacesController(IHttpClientFactory httpClientFactory)
        {
            //_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClientFactory = httpClientFactory;
        }
        */

        private readonly HttpClient _client;

        string _clientId;
        string _clientSecret;
        string _v = "20200617";

        public FoursquarePlacesController(HttpClient httpClient)
        {
            _client = httpClient;
            
            _clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            _clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");           
        }

        [FunctionName("Health")]
        public async Task<IActionResult> RunHealthCheck(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for Health Check.");

            try
            {
                var response = await _client.GetAsync($"https://api.foursquare.com/v2/venues/search?client_id={_clientId}&client_secret={_clientSecret}&v={_v}&near=new york&intent=browse&radius=10000&limit=10");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                return new OkObjectResult(HealthCheckResult.Unhealthy("An unhealthy result.", exception));
            }

            return new OkObjectResult(HealthCheckResult.Healthy("A healthy result."));
        }

        [FunctionName("Venues")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var latQueryParameter = req.Query["lat"].ToString().Trim('f', 'F');
            if(string.IsNullOrEmpty(latQueryParameter))
                return new BadRequestObjectResult("Please pass a lat parameter");

            var lonQueryParameter = req.Query["lon"].ToString().Trim('f', 'F');
            if (string.IsNullOrEmpty(lonQueryParameter))
                return new BadRequestObjectResult("Please pass a lon parameter");

            string radius = "10000";
            var radiusQueryParameter = req.Query["radius"].ToString();
            if (!string.IsNullOrEmpty(radiusQueryParameter))
                radius = radiusQueryParameter;

            //https://api.foursquare.com/v2/venues/search?client_id={{client_id}}&client_secret={{client_secret}}&v={{v}}&near=new york&intent=browse&radius=10000&query=peter luger steak house&limit=10

            VenuesResponse result;            
            //string jsonContent;

            try
            {
                //var response = await _client.GetAsync($"https://api.foursquare.com/v2/venues/search?client_id={_clientId}&client_secret={_clientSecret}&v={_v}&near=new york&intent=browse&radius=10000&query=peter luger steak house&limit=10");                
                var response = await _client.GetAsync($"https://api.foursquare.com/v2/venues/search?client_id={_clientId}&client_secret={_clientSecret}&v={_v}&ll={latQueryParameter},{lonQueryParameter}&intent=browse&radius={radius}");
                result = await response.Content.ReadAsAsync<VenuesResponse>();
                //jsonContent = await response.Content.ReadAsStringAsync();// this seemed slower than reading into VenuesResponse
            }
            catch (Exception exception)
            {
                throw exception;
            }            

            return new OkObjectResult(result);
        }
    }
}
