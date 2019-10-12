using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using System.Text.RegularExpressions;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Text;

namespace backendFunctions
{
    public class dbFormat
    {
        public dbFormat(string qMsg)
        {
            try
            {
                byte[] data = Convert.FromBase64String(qMsg);
                string decodedString = Encoding.UTF8.GetString(data);
                //decodedString = decodedString.Replace("\"", "'");

                var desrialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);
                this.id = desrialized["id"];
                this.date = DateTime.Parse(desrialized["date"]);
                this.body = desrialized["body"];
                this.link = desrialized["link"];
                this.name = desrialized["name"];
                this.profile = desrialized["profile"];
                this.likes = desrialized["likes"];
                this.rts = desrialized["rts"];

            }
            catch (Exception e)
            {
                //throw new Exception($"{e}, {this.id}, {this.date.ToString()}, {this.body}, {this.link} ");
            }
        }
        public string id { get; set; }
        public DateTime date { get; set; }
        public string body { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string likes { get; set; }
        public string rts { get; set; }
        public string profile { get; set; }
    }

    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        private readonly string apiKey;

        public ApiKeyServiceClientCredentials(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }

    public class QueueTriggerNewTweets
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

        public QueueTriggerNewTweets()
        {
            this.init();
        }
        public async void init()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/date");
        }

        [FunctionName("QueueTriggerNewTweets")]
        public async void Run([QueueTrigger("tweets", Connection = "QueueStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed for: {myQueueItem}");
            // convert to object we want
            dbFormat item = new dbFormat(myQueueItem);

            // log.LogInformation(item.name + "   " + item.profile);

            // test the body to see if its naughty or nice ;)
            if (item.body!=null && this.GetSentiment(key_var, endpoint_var, item.body, log))
            {
                log.LogInformation("good sentiment.");
                // add to cosmos!
                try
                {
                    // Read the item to see if it exists.  
                    ItemResponse<dbFormat> andersenFamilyResponse = await this.container.ReadItemAsync<dbFormat>(item.id, new PartitionKey(null));
                    log.LogInformation("Item in database with id: {0} already exists\n", item.id);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                    ItemResponse<dbFormat> andersenFamilyResponse = await this.container.CreateItemAsync<dbFormat>(item, null);

                    // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                    log.LogInformation("Created item in database ");
                }
            }
        }

        public bool GetSentiment(string subscriptionKey, string endpoint, string tweet_body, ILogger log)
        {
            try
            {
                if (null == subscriptionKey)
                {
                    throw new Exception("Please set/export the environment variable: " + key_var);
                }
                if (null == endpoint)
                {
                    throw new Exception("Please set/export the environment variable: " + endpoint_var);
                }

                var credentials = new ApiKeyServiceClientCredentials(subscriptionKey);
                TextAnalyticsClient client = new TextAnalyticsClient(credentials)
                {
                    Endpoint = endpoint
                };

                // change body to get rid of unicode shit
                var s = Regex.Replace(tweet_body, @"[^\u0020-\u007E]", " ");
                log.LogInformation(s);
                var result = client.Sentiment(s, "en");
                if (result.Score >= .5 )
                    return true;
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

    }
}
