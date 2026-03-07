using BetFounders.CentralBackend.App.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.BootstrapCentralBackendServices(builder.Configuration);

var app = builder.Build();

await app.BootstrapDatabaseAsync();

app.BootstrapCentralBackendPipeline();

app.Run();