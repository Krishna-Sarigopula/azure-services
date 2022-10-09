using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

internal class Program
{
    public class Order
    {
        private List<Order> _orders;

        public List<Order> Orders
        {
            get
            {
                return _orders;
            }
        }

        public Order()
        {
            _orders = new List<Order>();
            _orders.Add(new Order(1, 100));
            _orders.Add(new Order(2, 200));
            _orders.Add(new Order(3, 300));
        }

        public Order(int orderId, int quantity)
        {
            OrderId = orderId;
            Quantity = quantity;
        }

        public int OrderId { get; set; }

        public int Quantity { get; set; }
    }

    static string connectionString = "Endpoint=sb://myadda.servicebus.windows.net/;SharedAccessKeyName=all;SharedAccessKey=be7nj0nFBRkeNwLvGaBdFjozKD4pjdDlzx7/oMZJQzc=;EntityPath=order";
    static string topicName = "order";
    //static string queueName = "orders/$DeadLetterQueue";  in case receive dead letter queue
    static Order order = new Order();
    static string subscription1 = "sub1";
    static string subscription2 = "sub2";

    private static async Task Main(string[] args)
    {
        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var sender = senderClient.CreateProcessor(topicName, subscription1, new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock // only received by one receiver & locked 
        });

        sender.ProcessMessageAsync += Sub1ReceiverProcessor;
        sender.ProcessErrorAsync += ErrorHandler;

        var sender1 = senderClient.CreateProcessor(topicName, subscription2, new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        sender1.ProcessMessageAsync += Sub2ReceiverProcessor;
        sender1.ProcessErrorAsync += ErrorHandler;

        await sender1.StartProcessingAsync();
        await sender.StartProcessingAsync();

        sendMsgsToTopic();
        Console.WriteLine("Hello, World!");

        Console.ReadKey();
    }

    public static void sendMsgsToTopic()
    {
        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var sender = senderClient.CreateSender(topicName);

        ServiceBusMessageBatch busMessageBatch = sender.CreateMessageBatchAsync().Result;

        foreach (Order order in order.Orders)
        {
            var msg = JsonConvert.SerializeObject(order);
            var servicebusMsg = new ServiceBusMessage(msg);
            servicebusMsg.ContentType = "application/json"; // can mention other types like text,xml
            servicebusMsg.TimeToLive = TimeSpan.FromSeconds(10); // to set TTL
            servicebusMsg.MessageId = "1"; // to detect the duplicate detection
            busMessageBatch.TryAddMessage(servicebusMsg);
        }

        Console.WriteLine("Sending Messages");
        sender.SendMessagesAsync(busMessageBatch);

        senderClient.DisposeAsync();
        sender.DisposeAsync();
    }

    public static async Task Sub1ReceiverProcessor(ProcessMessageEventArgs eventArgs)
    {
        Console.WriteLine("Message received From Sub1");
        Console.WriteLine(eventArgs.Message.Body);
        await eventArgs.CompleteMessageAsync(eventArgs.Message);
    }

    public static async Task Sub2ReceiverProcessor(ProcessMessageEventArgs eventArgs)
    {
        Console.WriteLine("Message received From Sub2");
        Console.WriteLine(eventArgs.Message.Body);
        await eventArgs.CompleteMessageAsync(eventArgs.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}