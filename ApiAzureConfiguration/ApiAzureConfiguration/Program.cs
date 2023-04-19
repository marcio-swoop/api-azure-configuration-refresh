using ApiAzureConfiguration.Extensions;
using static ApiAzureConfiguration.Extensions.CorsExtensions;

var builder = WebApplication.CreateBuilder(args);

//
// Azure AppConfiguration
//
builder.ConfigureAzureAppConfigRefresher();

//
// Configuration FROM Azure
//
builder.ConfigureCors();

//
// Add services to the container.
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//
// Configure the HTTP request pipeline.
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAzureAppConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
