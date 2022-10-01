using FirstWebApp.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        // this below setting loading from Azure web apps configuration/connection string settings
        var connectionString = builder.Configuration.GetConnectionString("DbConnectionString");

        //configuring the App Configuration service inside azure as seperate service but not in appservice
        var azureApplicationConfigurationConnectionString = "Endpoint=https://myaddaappconfig.azconfig.io;Id=mp59-l4-s0:kHn/VtsxHuJhOKnnTDpM;Secret=S4Z2/Q40EjbzYzSw6cOd4DQJXXqha/XU9qXgez9GFII=";
        builder.Host.ConfigureAppConfiguration(app =>
        {
            app.AddAzureAppConfiguration(azureApplicationConfigurationConnectionString);
        });
        connectionString = builder.Configuration["DbConnectionString"];

        builder.Services.AddDbContextPool<AppDbContext>(
            x => x.UseSqlServer(connectionString));
        builder.Services.AddFeatureManagement(); //feature settings from azure coming from azure app configuration service

        builder.Services.AddAzureAppConfiguration();
        builder.Services.AddApplicationInsightsTelemetry("817ea276-b458-443d-b3ba-b249703d4404");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

       // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;
                await Task.FromResult(true);
                // log the exception etc..
                // produce some response for the caller
            });
        });

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}