using System;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues.Specialized;

internal class Program
{
    static string CONNECTION_STRING = "SharedAccessSignature=sv=2021-04-10&ss=q&srt=sco&st=2022-10-02T03%3A44%3A54Z&se=2022-10-03T03%3A44%3A54Z&sp=rwdxftlacup&sig=nXQ%2FQ%2FQFsgXd50IMfE0jN1jpfECvMi36trRfJjB%2FAZg%3D;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";
    private static void Main(string[] args)
    {
        var client = CreateQueueClient();
        SendMessage(client, "Hello, World!");
        UpdateMessage(client, "Hello, World updated!");
        Thread.Sleep(2000);
        ReadMessage(client);
        ReadMessageWithOptions(client);
        DeQueueMessage(client);
        QueueLength(client);
        DeleteQueue(client);
        Console.ReadLine();
    }

    public static QueueClient CreateQueueClient()
    {
        QueueClient queueClient = new QueueClient(CONNECTION_STRING, "test");
        queueClient.CreateIfNotExists();
        return queueClient;
    }

    public static void SendMessage(QueueClient queueClient, string message)
    {
        queueClient.SendMessage(message);
    }

    public static void UpdateMessage(QueueClient queueClient, string message)
    {
        QueueMessage[] msgs = queueClient.ReceiveMessages();
        queueClient.UpdateMessage(msgs[0].MessageId, msgs[0].PopReceipt, message);
    }

    public static void ReadMessage(QueueClient queueClient)
    {
        var msg = queueClient.ReceiveMessage();
        Console.WriteLine(msg.Value.Body);
    }

    public static void ReadMessageWithOptions(QueueClient queueClient)
    {
        QueueMessage[] msgs = queueClient.ReceiveMessages(20, TimeSpan.FromMinutes(5));
        foreach (QueueMessage message in msgs)
        {
            // Process (i.e. print) the messages in less than 5 minutes
            Console.WriteLine($"De-queued message: '{message.Body}'");
        }
    }

    public static void DeQueueMessage(QueueClient queueClient)
    {
        QueueMessage[] msgs = queueClient.ReceiveMessages();
        queueClient.DeleteMessage(msgs[0].MessageId, msgs[0].PopReceipt);
    }

    public static void QueueLength(QueueClient queueClient)
    {
        QueueProperties props = queueClient.GetProperties();
        Console.WriteLine($"Number of messages in queue: {props.ApproximateMessagesCount}");
    }

    public static void DeleteQueue(QueueClient queueClient)
    {
        queueClient.Delete();
    }
}