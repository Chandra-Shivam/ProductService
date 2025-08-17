using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DBContext;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri("https://secretskeyvalut.vault.azure.net/"),
    new DefaultAzureCredential()
);

string postgreConnString = builder.Configuration["PostgreConnStr"];
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(postgreConnString));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"], TelemetryConverter.Traces)
    .CreateLogger();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers();
builder.Services.AddScoped<DbServiceLayer>();

var app = builder.Build();
app.UseMetricServer();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();
app.Run();