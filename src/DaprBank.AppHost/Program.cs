using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis for Dapr state store and pub/sub
var redis = builder.AddRedis("redis");

// Add the BankService using the project path
var bankService = builder.AddProject("bankservice", "../BankService/BankService.csproj")
    .WithReference(redis);

builder.Build().Run();