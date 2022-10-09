using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

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

    static string connectionString = "Endpoint=sb://myadda.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xMP2V9f375RhYDtWLiO6EfNpMjvbTz+NNt1pj/P88IA=";
    static string queueName = "orders";
    static Order order = new Order();

    private static async Task Main(string[] args)
    {

        //PeekMessage();
        //ReceiveMessage();

        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var sender = senderClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock // only received by one receiver & locked 
        });

        sender.ProcessMessageAsync += ReceiverProcessor;
        sender.ProcessErrorAsync += ErrorHandler;

        var sender1 = senderClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        sender1.ProcessMessageAsync += AnotherReceiverProcessorForPeeklock;
        sender1.ProcessErrorAsync += ErrorHandler;

        await sender1.StartProcessingAsync();
        await sender.StartProcessingAsync();


        sendMsgsToQueue();

        Console.ReadKey();
    }

    public static void sendMsgsToQueue()
    {
        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var sender = senderClient.CreateSender(queueName);

        ServiceBusMessageBatch busMessageBatch = sender.CreateMessageBatchAsync().Result;

        foreach (Order order in order.Orders)
        {
            var msg = JsonConvert.SerializeObject(order);
            var servicebusMsg = new ServiceBusMessage(msg);
            servicebusMsg.ContentType = "application/json"; // can mention other types like text,xml
            servicebusMsg.TimeToLive = TimeSpan.FromSeconds(10); // to set TTL
            busMessageBatch.TryAddMessage(servicebusMsg);
        }

        Console.WriteLine("Sending Messages");
        sender.SendMessagesAsync(busMessageBatch);

        senderClient.DisposeAsync();
        sender.DisposeAsync();
    }

    public static async void PeekMessage()
    {
        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var receiver = senderClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

        var msgs = receiver.ReceiveMessagesAsync();

        await foreach (var item in msgs)
        {
            Console.WriteLine(item.Body);
        }

        await senderClient.DisposeAsync();
        await receiver.DisposeAsync();
    }

    public static async void ReceiveMessage()
    {
        ServiceBusClient senderClient = new ServiceBusClient(connectionString);
        var receiver = senderClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

        var msgs = receiver.ReceiveMessagesAsync();

        await foreach (var item in msgs)
        {
            Console.WriteLine(item.Body);

            foreach (var prop in item.ApplicationProperties)
            {
                Console.WriteLine($"{prop.Key} {prop.Value}");
            }
        }

        await senderClient.DisposeAsync();
        await receiver.DisposeAsync();
    }

    public static async Task ReceiverProcessor(ProcessMessageEventArgs eventArgs)
    {
        Console.WriteLine("Message received");
        Console.WriteLine(eventArgs.Message.Body);
        await eventArgs.CompleteMessageAsync(eventArgs.Message);
    }

    public static async Task AnotherReceiverProcessorForPeeklock(ProcessMessageEventArgs eventArgs)
    {
        Console.WriteLine("Message received From Second Receiver");
        Console.WriteLine(eventArgs.Message.Body);
        await eventArgs.CompleteMessageAsync(eventArgs.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

}