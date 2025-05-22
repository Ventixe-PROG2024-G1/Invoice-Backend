using EmailServiceProvider.Services;
using Invoice.Business.Builder;
using Invoice.Business.Models;
using Invoice.Business.Services;
using Invoice.Data.Context;
using Invoice.Data.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ServiceBusReceiveSettings>(builder.Configuration.GetSection("AzureServiceBus:Receive"));
builder.Services.Configure<ServiceBusSendSettings>(builder.Configuration.GetSection("AzureServiceBus:Send"));

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

builder.Services.AddGrpcClient<EmailContract.EmailContractClient>(o =>
{
    o.Address = new Uri("https://localhost:7246");
});


builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceServices,InvoiceServices>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();
app.Run();
