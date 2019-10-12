using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using fakedata;
using System.Linq;

namespace CosmosGettingStartedTutorial
{
    class Program
    {
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

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...");
                Program p = new Program();
                await p.GetStartedDemoAsync();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Entry point to call methods that operate on Azure Cosmos DB resources in this sample
        /// </summary>
        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync();
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/date");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        /// <summary>
        /// Add Family items to the container
        /// </summary>
        private async Task AddItemsToContainerAsync()
        {
            //rng
            Random gen = new Random();


            for (int i=2; i<10000; i++)
            {
                // get rand date
                DateTime start = new DateTime(2019, 10, 1);
                int range = ((TimeSpan)(DateTime.Today - start)).Days;
                DateTime dd = start.AddDays(gen.Next(range));


                // make data
                tweet_data d = new tweet_data
                {
                    id = i.ToString(),
                    body = "body",
                    date = dd,
                    likes = gen.Next(10000).ToString(),
                    rts = gen.Next(10000).ToString(),
                    link = "www.google.com",
                    name = "fake",
                    profile = "https://vignette.wikia.nocookie.net/pixar/images/3/38/Mike1.png/revision/latest/scale-to-width-down/310?cb=20170807223616"
                };

                try
                {
                    // Read the item to see if it exists.  
                    ItemResponse<tweet_data> andersenFamilyResponse = await this.container.ReadItemAsync<tweet_data>(d.id, new PartitionKey(null));
                    Console.WriteLine("Item in database with id: {0} already exists\n", d.id);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                    ItemResponse<tweet_data> andersenFamilyResponse = await this.container.CreateItemAsync<tweet_data>(d, null);

                    // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                    Console.WriteLine("Created item in database ");
                }
            }
            
        }
    }
}