var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var sqlServer = builder.AddSqlServer("sql-server", sqlPassword, port: 1433)
    .WithDataVolume()
    .AddDatabase("authservice-command");

var postgres = builder.AddPostgres("postgres", postgresPassword, port: 5432)
    .WithDataVolume()
    .AddDatabase("authservice-query");

var api = builder.AddProject("authservice-api", @"..\..\src\AuthService.API\AuthService.API.csproj")
    .WithReference(sqlServer)
    .WithReference(postgres);

builder.Build().Run();