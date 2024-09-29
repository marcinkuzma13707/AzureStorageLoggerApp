using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using JokeStorageFunctionApp.Services;
using JokeStorageFunctionApp.Interfaces;
using System;

[assembly: FunctionsStartup(typeof(JokeStorageFunctionApp.Startup))]

namespace JokeStorageFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register TableServiceClient with your connection string
            builder.Services.AddSingleton((s) =>
            {
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                return new TableServiceClient(connectionString);
            });

            // Register LogService with the table name you are using
            builder.Services.AddSingleton<ILogService>(s =>
            {
                var tableServiceClient = s.GetRequiredService<TableServiceClient>();
                return new LogService(tableServiceClient, "LogsTable"); // Replace "LogsTable" with your actual table name
            });

            builder.Services.AddHttpClient();
        }
    }
}
