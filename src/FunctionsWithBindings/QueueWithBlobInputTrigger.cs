using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionsWithBindings
{
    public class QueueWithBlobInputTrigger
    {

        // {queueTrigger} is binds the message from queue message automatically . it is syetm defined keyword
        [FunctionName("QueueWithBlobInputTrigger")]
        public void Run([QueueTrigger("%input_queue_name1%")]string myQueueItem,
            [Blob("pics/{queueTrigger}", FileAccess.Read)] Stream stream,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {stream.Length}");
        }
    }
}
