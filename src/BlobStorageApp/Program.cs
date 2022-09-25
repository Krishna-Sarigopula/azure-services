using Azure.Storage.Blobs;
using BlobStorageApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var blobClient = new BlobServiceClient("SharedAccessSignature=sv=2021-04-10&ss=btqf&srt=sco&st=2022-09-25T07%3A29%3A39Z&se=2022-09-26T07%3A29%3A39Z&sp=rwdl&sig=lZIiDfsor%2FRVNSct3btsdmFj%2B%2FhO%2FZ7SOh9HuJXJzL0%3D;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;");
builder.Services.AddSingleton(blobClient);
builder.Services.AddSingleton(d => new StorageService(blobClient));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
