var builder = DistributedApplication.CreateBuilder(args);


var username = builder.AddParameter("mongo-username");
var password = builder.AddParameter("mongo-password");
var mongo = builder.AddMongoDB("mongo", 27017)
                    .WithDataVolume() // Adds a data volume to store models
                    .WithDbGate();  // Mongo web UI
                                    //.WithLifetime(ContainerLifetime.Persistent);     // keep container alive

var ollama = builder.AddOllama("ollama", 11434)
                    .WithImageTag("latest")
                    .WithDataVolume() // Adds a data volume to store models
                    .WithGPUSupport(); // Enable GPU support
                    //.WithLifetime(ContainerLifetime.Persistent); // Keep the container running

//var api = builder.AddProject<Projects.LangMate_AppHost_ApiService>("langmate-api")
//                .WithHttpHealthCheck("/health")
//                .WithHttpEndpoint(name: "external", port: 5001)
//                .WithReference(mongo)
//                .WaitFor(mongo)
//                .WithReference(ollama)
//                .WaitFor(ollama);

builder.AddProject<Projects.LangMate_AppHost_BlazorUI>("langmate-ui")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        //.WithReference(api)
        //.WaitFor(api)
        .WithReference(mongo)
        .WaitFor(mongo)
        .WithReference(ollama)
        .WaitFor(ollama);

builder.Build().Run();
