using LangMate.Core;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddLangMateCore(builder.Configuration, useApm: false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseLangMateCore(app.Configuration, loggerFactory);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
