using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionsWithBindings
{
    public class QueToQueTrigger
    {
        [FunctionName("QueToQueTrigger")]
        public void Run([QueueTrigger("%input_queue_name4%")]string myQueueItem,
            [Queue("%input_queue_name2%")] out string queue,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            queue = string.Empty;
        }
    }
}
