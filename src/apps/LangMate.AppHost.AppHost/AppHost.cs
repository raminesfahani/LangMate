var builder = DistributedApplication.CreateBuilder(args);

// Getting MongoDB parameters from env
var username = builder.AddParameter("mongo-username");
var password = builder.AddParameter("mongo-password");

var mongo = builder.AddMongoDB("mongo", 27017)
                    .WithDataVolume("mongo-data")
                    .WithDbGate();

var ollama = builder.AddOllama("ollama", 11434)
                    .WithImageTag("latest")
                    .WithDataVolume("ollama-data")
                    .WithGPUSupport();

// Optional
//ollama.AddModel("llama3"); // Download a sample ollama model

builder.AddProject<Projects.LangMate_AppHost_ApiService>("langmate-api")
                .WithHttpHealthCheck("/health")
                .WithHttpEndpoint(name: "external", port: 5001)
                .WithReference(mongo)
                .WaitFor(mongo)
                .WithReference(ollama)
                .WaitFor(ollama);

builder.AddProject<Projects.LangMate_AppHost_BlazorUI>("langmate-ui")
        .WithExternalHttpEndpoints()
        .WithHttpHealthCheck("/health")
        .WithReference(mongo)
        .WaitFor(mongo)
        .WithReference(ollama)
        .WaitFor(ollama);

builder.Build().Run();
