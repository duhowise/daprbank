using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis for Dapr state store and pub/sub
var redis = builder.AddRedis("redis");

// Add the BankService 
// Note: When Dapr CLI is available, this can be enhanced to run with Dapr sidecar
// using Community Toolkit extensions or manual configuration
var bankService = builder.AddProject("bankservice", "../BankService/BankService.csproj")
    .WithReference(redis)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DAPR_HTTP_PORT", "3500")
    .WithEnvironment("DAPR_GRPC_PORT", "50001");

// TODO: When CommunityToolkit.Aspire.Hosting.Dapr is properly configured:
// var bankService = builder.AddProject("bankservice", "../BankService/BankService.csproj")
//     .WithReference(redis)
//     .WithDaprSidecar(new DaprSidecarOptions 
//     {
//         AppId = "bankservice",
//         ComponentsPath = "../../components",
//         Config = "../../dapr-config.yaml"
//     });

builder.Build().Run();