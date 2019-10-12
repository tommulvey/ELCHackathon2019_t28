using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace backendFunctions
{
    public class metrics
    {
        private const string key_var = "1e85ee13748b4c61b6e17676ce1868d2";
        private static readonly string subscriptionKey = Environment.GetEnvironmentVariable(key_var);
        private const string endpoint_var = "https://text-sentiments.cognitiveservices.azure.com/";
        private static readonly string endpoint = Environment.GetEnvironmentVariable(endpoint_var);
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://team28.documents.azure.com/";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "lVLk1cd6W79qgTbp7DAfs9QoLxFuewOsgVYLrXRHP9QOufaVdbv3IH274fa9gDYKpyoYBg3RlTF3bLjWvTq2TA==";
        // The Cosmos client instance
        private CosmosClient cosmosClient;
        // The database we will create
        private Database database;
        // The container we will create.
        private Container container;
        // The name of the database and container we will create
        private string databaseId = "tweets";
        private string containerId = "tweets";

        public metrics()
        {
            this.init();
        }
        public async void init()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/date");
        }
        [FunctionName("metrics")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "month")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger metrics");
            string month = req.Query["month"];



            return (ActionResult)new OkObjectResult("ok!" + month);
        }
    }
}
