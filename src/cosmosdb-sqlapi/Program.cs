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
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: 12,
                sale: false
            );

            
        var items = new List<Product>(){
new Product(
                id: "1871951839129",
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: 12,
                sale: false
            ),
            new Product(
                id: "2871951839121",
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: 12,
                sale: false
            ),
             new Product(
                id: "3871951839121",
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: -1,
                sale: false
            )
        };

        DeleteItem("687195183911");

        CreateItem(newItem);

        InsertItemsProcedure(items);

       var item123 =  new Product(
                id: "3871951839121676",
                category: "gear-surf-surfboards",
                name: "Yamba Surfboard",
                quantity: -1,
                sale: false
            );
        InsertItemsProcedureFunc(item123);

        UpsertItem(newItem);

        GetProduct(newItem.id);

        GetProductbyQuery();

        Console.ReadKey();
    }

    private static void SetClient()
    {
        var endPoint = "https://myaddacosmosdb.documents.azure.com:443/";
        var token = "2z8dUSopXcZ8b6IR8VWsAw8bsXanMulh74ABPtRtsafEMUTNw6KBmQvve5lkAuwdNsBGbxUtBeYK2NEe0dkT0Q==";
        cosmosClient = new CosmosClient(endPoint, token);
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

    private static void DeleteItem(string id)
    {
        Product item = container.DeleteItemAsync<Product>(id, new PartitionKey("gear-surf-surfboards")).Result;
        Console.WriteLine($"Deleted item");
    }

    private static void CreateItem(Product product)
    {
        Product item = container.CreateItemAsync<Product>(product).Result;
        Console.WriteLine($"Created item:\t{item.id}\t[{item.category}]");
    }

    //upsert instead of create a new item in case you run this sample code more than once.
    private static void UpsertItem(Product product)
    {
        Product item = container.UpsertItemAsync<Product>(product, new PartitionKey("gear-surf-surfboards")).Result;
        Console.WriteLine($"Created item:\t{item.id}\t[{item.category}]");
    }

    private static void InsertItemsProcedure(List<Product> items){
        var res = container.Scripts.ExecuteStoredProcedureAsync<string>("spInsertProc", new PartitionKey("gear-surf-surfboards"), new [] {items}).Result;
    }

    private static void InsertItemsProcedureFunc(Product items){
        var res = container.CreateItemAsync<Product>(items,new PartitionKey("gear-surf-surfboards"),
        new ItemRequestOptions() {PreTriggers = new List<string>() {"preTrigger"}}).Result;
    }

    private static void GetProduct(string id)
    {
        Product item = container.ReadItemAsync<Product>(id, new PartitionKey("gear-surf-surfboards")).Result;
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
