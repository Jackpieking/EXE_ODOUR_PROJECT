using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args: args);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
