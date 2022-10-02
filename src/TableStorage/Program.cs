
// New instance of the TableClient class
using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

internal class Program
{
    private static void Main(string[] args)
    {

        var prod1 = new Product()
        {
            RowKey = "68719518388",
            PartitionKey = "Mobiles",
            Name = "Nokia",
            Quantity = 8,
            Sale = true
        };

        var prod2 = new Product()
        {
            RowKey = "68719518389",
            PartitionKey = "Mobiles",
            Name = "Apple",
            Quantity = 8,
            Sale = true
        };

        TableClient tableClient = GetClient();
        AddEntity(tableClient, prod1);
        AddEntity(tableClient, prod2);

        var prod = GetEntityByRowKey(tableClient, "68719518388", "Mobiles");
        var prods = GetEntityByQuery(tableClient, "Mobiles");

        var json = JsonConvert.SerializeObject(prod);
        Console.Write(json);
        Console.WriteLine("=========================", Environment.NewLine);
        foreach (var item in prods)
        {
            json = JsonConvert.SerializeObject(item);
            Console.Write(json);
        }

        Console.ReadKey();
    }

    public static TableClient GetClient()
    {
        TableServiceClient tableServiceClient = new TableServiceClient("SharedAccessSignature=sv=2021-04-10&ss=t&srt=sco&st=2022-10-02T10%3A15%3A08Z&se=2022-10-03T10%3A15%3A08Z&sp=rwdxftlacup&sig=dM1lcdwGeq%2BWTDhAZB3KnKR8Il18fd2PUJ692dotAhM%3D;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;");
        var tableClient = tableServiceClient.GetTableClient("products");
        tableClient.CreateIfNotExists();
        return tableClient;
    }

    public static void AddEntity(TableClient tableClient, Product product)
    {
        tableClient.AddEntity<Product>(product);
    }

    public static Product GetEntityByRowKey(TableClient tableClient, string rowKey, string partionKey)
    {
        return tableClient.GetEntity<Product>(partionKey, rowKey);
    }

    public static IEnumerable<Product> GetEntityByQuery(TableClient tableClient, string partionKey)
    {
        return tableClient.Query<Product>(x => x.PartitionKey == partionKey);
    }

    public record Product : ITableEntity
    {
        public string RowKey { get; set; } = default!;

        public string PartitionKey { get; set; } = default!;

        public string Name { get; init; } = default!;

        public int Quantity { get; init; }

        public bool Sale { get; init; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
}