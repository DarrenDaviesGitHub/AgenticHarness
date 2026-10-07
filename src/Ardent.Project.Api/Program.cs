using Ardent.Project.Api;
using Ardent.Project.Application;
using Ardent.Project.Infrastructure;
using Ardent.Project.Infrastructure.Persistence;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration["MediatR:LicenseKey"])
    .AddInfrastructure()
    .AddPresentation();

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}
else
{
    app.UseHsts();
}

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
