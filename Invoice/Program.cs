using Invoice.Business.Builder;
using Invoice.Business.Models;
using Invoice.Business.Services;
using Invoice.Data.Context;
using Invoice.Data.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.Configure<BookingServiceBusSettings>(builder.Configuration.GetSection("BookingServiceBus"));
builder.Services.Configure<InvoiceServiceBusSettings>(builder.Configuration.GetSection("InvoiceServiceBus"));
builder.Services.Configure<EmailServiceBusSettings>(builder.Configuration.GetSection("EmailServiceBus"));


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 100;
});
builder.Services.AddHostedService<ServiceBusListenerServices>();
builder.Services.AddSingleton<IServiceBusPublishService, ServiceBusPublishService>();
builder.Services.AddScoped<IInvoiceHtmlBuilder, InvoiceHtmlBuilder>();
builder.Services.AddScoped<IInvoicePlainTextBuilder, InvoicePlainTextBuilder>();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceServices,InvoiceServices>();
builder.Services.AddScoped<IInvoiceEmailService, ServiceBusEmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

const string apiKeyHeader = "X-API-KEY";
var configuredApiKey = builder.Configuration["ApiSettings:ApiKey"];

app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue(apiKeyHeader, out var extractedApiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API-nyckel saknas.");
        return;
    }

    if (!configuredApiKey.Equals(extractedApiKey))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Ogiltig API-nyckel.");
        return;
    }

    await next();
});

app.MapOpenApi();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();
app.Run();
