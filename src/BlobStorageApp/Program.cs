using Azure.Core.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Keys;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using BlobStorageApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string tenantId = "d7713ac2-b668-4dbb-b7c8-e49fadcf1c81";
string clientId = "31ead1a1-21ae-4fcb-bcf8-a19e0c089544";
string clientSecret = "ECz8Q~4KTLndTGa3Vi2RYp-0qmmeop-bpsIU6aS0";
ClientSecretCredential cred = new ClientSecretCredential(tenantId, clientId, clientSecret);
var vaultUri = new Uri("https://myadda.vault.azure.net/");
KeyClient keyClient = new KeyClient(vaultUri, cred);
KeyVaultKey rasKey = keyClient.GetKeyAsync("uploadkey").Result;
IKeyEncryptionKey key = new CryptographyClient(rasKey.Id, cred);
IKeyEncryptionKeyResolver keyResolver = new KeyResolver(cred);

// Create the encryption options to be used for upload and download.
ClientSideEncryptionOptions encryptionOptions = new ClientSideEncryptionOptions(ClientSideEncryptionVersion.V2_0)
{
    KeyEncryptionKey = key,
    KeyResolver = keyResolver,
    // String value that the client library will use when calling IKeyEncryptionKey.WrapKey()
    KeyWrapAlgorithm = "some algorithm name"
};

BlobClientOptions options = new SpecializedBlobClientOptions() { ClientSideEncryption = encryptionOptions };

var blobClient = new BlobServiceClient("SharedAccessSignature=sv=2021-04-10&ss=btqf&srt=sco&st=2022-10-01T15%3A46%3A45Z&se=2022-10-02T15%3A46%3A45Z&sp=rwdxl&sig=xk%2BkoyBpDV%2Fv%2BMfyTWrC%2FzYxnjega8Z1Kfx80S7HerQ%3D;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;", options);
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
