using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionsWithBindings
{
    public class QueueTrigger
    {
        [FunctionName("Function1")]
        //%input_queue_name% is binding expression coming from appsettings in azure local.settings file Values section in local 
        public void Run([QueueTrigger("%input_queue_name%")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
