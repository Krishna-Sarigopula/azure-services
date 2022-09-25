using System;
using System.IO;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionsWithBindings
{
    public class QueueWithBlobOutputTrigger
    {
        [FunctionName("QueueWithBlobOutputTrigger")]
        public void Run([QueueTrigger("%input_queue_name2%")]string myQueueItem,
            [Blob("pics/acc.txt",FileAccess.Write)] Stream stream,
            ILogger log)
        {
            stream.Write(Encoding.ASCII.GetBytes(myQueueItem));

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
