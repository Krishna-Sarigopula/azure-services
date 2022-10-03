using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;
using static Program;

internal class Program
{
    private static CosmosClient cosmosClient = default!;
    private static Database res = default!;
    private static Container container = default!;

    private static void Main(string[] args)
    {
        SetClient();
        CreateDatabase();
        CreateContainer();

        Product newItem = new(
                id: "687195183911",
                category: "gear1-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: 12,
                sale: false
            );

        CreateItem(newItem);

        UpsertItem(newItem);

        GetProduct();

        GetProductbyQuery();

        Console.ReadKey();
    }

    private static void SetClient()
    {
        cosmosClient = new CosmosClient("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
    }

    private static void CreateDatabase()
    {
        res = cosmosClient.CreateDatabaseIfNotExistsAsync("ecommerce").Result;
        Console.WriteLine("Created Database: {0}\n", res.Id);
    }

    private static void CreateContainer()
    {
        container = res.CreateContainerIfNotExistsAsync("orders", "/category").Result;
        Console.WriteLine("Created Container: {0}\n", container.Id);
    }

    private static void CreateItem(Product product)
    {
        Product item = container.CreateItemAsync<Product>(product).Result;
        Console.WriteLine($"Created item:\t{item.id}\t[{item.category}]");
    }

    //upsert instead of create a new item in case you run this sample code more than once.
    private static void UpsertItem(Product product)
    {
        Product item = container.UpsertItemAsync<Product>(product, new PartitionKey("gear1-surf-surfboards")).Result;
        Console.WriteLine($"Created item:\t{item.id}\t[{item.category}]");
    }

    private static void GetProduct()
    {
        Product item = container.ReadItemAsync<Product>("68719518391", new PartitionKey("gear-surf-surfboards")).Result;
        Console.WriteLine($"Read item:\t{item.id}\t[{item.category}]");
    }

    private static void GetProductbyQuery()
    {
        var query = new QueryDefinition(
    query: "SELECT * FROM products p WHERE p.category = @key").WithParameter("@key", "gear-surf-surfboards");

        using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
            queryDefinition: query
        );

        while (feed.HasMoreResults)
        {
            FeedResponse<Product> response = feed.ReadNextAsync().Result;
            foreach (Product item in response)
            {
                Console.WriteLine($"Found item:\t{item.name}");
            }
        }
    }

    public record Product(
    string id,
    string category,
    string name,
    int quantity,
    bool sale
);
}
