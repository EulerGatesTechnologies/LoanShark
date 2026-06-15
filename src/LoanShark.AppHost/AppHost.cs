var builder = DistributedApplication.CreateBuilder(args);

var sqldata = builder.AddSqlServer("sql")
                     .AddDatabase("sqldata");

var api = builder.AddProject<Projects.LoanShark_Api>("api")
                 .WithReference(sqldata)
                 .WaitFor(sqldata);

builder.AddProject<Projects.LoanShark_Web>("web")
       .WithExternalHttpEndpoints()
       .WithReference(api)
       .WaitFor(api);

builder.AddExecutable("maui-windows", "dotnet", "../LoanShark.Maui", "run", "-f", "net10.0-windows10.0.19041.0")
       .WithReference(api)
       .WaitFor(api);

builder.Build().Run();
