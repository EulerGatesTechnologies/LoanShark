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

builder.Build().Run();
