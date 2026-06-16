var builder = DistributedApplication.CreateBuilder(args);

var sqldata = builder.AddAzureSqlServer("sql")
                     .RunAsContainer()
                     .AddDatabase("sqldata");

var api = builder.AddProject<Projects.LoanShark_Api>("api")
                 .WithReference(sqldata)
                 .WaitFor(sqldata)
                 .WithExternalHttpEndpoints();

builder.AddProject<Projects.LoanShark_Web>("web")
       .WithExternalHttpEndpoints()
       .WithReference(api)
       .WaitFor(api);

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddExecutable("maui-windows", "dotnet", "../LoanShark.Maui", "run", "-f", "net10.0-windows10.0.19041.0")
           .WithReference(api)
           .WaitFor(api);
}

builder.Build().Run();
