using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using backendFunctions;
using System;

namespace backendFunctions
{
    public class metrics { 

        [FunctionName("metrics")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "stats/day/{day}")] HttpRequest req,
            [CosmosDB("tweets", "tweets",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from tweets t where t.day = {day}")]
                IEnumerable<Form> res,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger metrics");
                string day = req.Query["day"];
                string[] args = day.Split("-");
                // args [0] is year
                // args [1] month
                // args [2] day
                DateTime d = new DateTime(Int32.Parse(args[0]), Int32.Parse(args[1]), Int32.Parse(args[0]));

                

                return (ActionResult)new OkObjectResult("ok!" + day);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
