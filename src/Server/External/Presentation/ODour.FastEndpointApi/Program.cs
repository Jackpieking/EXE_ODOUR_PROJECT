using Microsoft.AspNetCore.Builder;
using ODour.PostgresRelationalDb;

var builder = WebApplication.CreateBuilder(args: args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddOdourPostgresRelationalDb(configuration: configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Run();
