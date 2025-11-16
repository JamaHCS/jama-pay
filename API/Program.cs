using API.extensions;
using Domain.Extensions;
using Repository.Extensions;
using Service.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Repository.Context;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddAutoMappers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddFluentValidations();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("jamaPayDB"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
