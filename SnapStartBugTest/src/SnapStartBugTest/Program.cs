using SnapshotRestoreService = Amazon.Lambda.Core.SnapshotRestore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Adding HttpContextAccessor result in error - https://gist.github.com/budgetdevv/b1304220f2cf3921723109a53f9a82a0
services.AddHttpContextAccessor();

services.AddAWSLambdaBeforeSnapshotRequest(
    new(HttpMethod.Get, "/warmup")
);

SnapshotRestoreService.RegisterBeforeSnapshot(async () =>
    Console.WriteLine("Before Snapshot!")
);

SnapshotRestoreService.RegisterAfterRestore(async () =>
    Console.WriteLine("Snapshot restored!")
);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");
app.MapGet("/warmup", () => "Warmed up!");

app.Run();