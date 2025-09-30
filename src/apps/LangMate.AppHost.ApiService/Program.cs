using LangMate.AppHost.ServiceDefaults;
using LangMate.Core;
using LangMate.Middleware.Serilog;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = LangMateLoggingExtensions.AddLangMateLogging(builder.Configuration);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddLangMateCore(builder.Configuration, useApm: false);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseLangMateCore(app.Configuration, loggerFactory);

app.MapDefaultEndpoints();
app.MapControllers();

app.Run();
