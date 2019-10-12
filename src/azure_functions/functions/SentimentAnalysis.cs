using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace functions
{
    public class PostData
    {
        public string TweetInfo { get; set; }
    }

    public static class SentimentAnalysis
    {
        [FunctionName("TweetsQueueTrigger")]
        public static void Run([QueueTrigger("tweets",
            Connection = "QueueStorage")]string myQueueItem, ILogger log)
        {
            // log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            
        }
    }

}
