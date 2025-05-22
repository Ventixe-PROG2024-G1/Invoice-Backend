using Azure.Messaging.ServiceBus;
using Invoice.Business.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Invoice.Business.Services
{
    public class ServiceBusListenerServices : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServiceBusListenerServices> _logger;

        public ServiceBusListenerServices(IOptions<ServiceBusReceiveSettings> sbOptions,IServiceScopeFactory scopeFactory,ILogger<ServiceBusListenerServices> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var client = new ServiceBusClient(sbOptions.Value.ConnectionString);
            _processor = client.CreateProcessor(
                queueName: sbOptions.Value.QueueName,
                new ServiceBusProcessorOptions
                {
                    MaxConcurrentCalls = 2,
                    AutoCompleteMessages = false,
                    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15)
                });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += OnMessageAsync;
            _processor.ProcessErrorAsync += OnErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("ServiceBusListener startad.");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }

            await _processor.StopProcessingAsync();
            _logger.LogInformation("ServiceBusListener stoppad.");
        }

        private async Task OnMessageAsync(ProcessMessageEventArgs args)
        {
            using var scope = _scopeFactory.CreateScope();
            var invoiceServices = scope.ServiceProvider.GetRequiredService<IInvoiceServices>();

            var body = args.Message.Body.ToString();
            _logger.LogInformation("Mottog meddelande: {MessageId}", args.Message.MessageId);

            try
            {
                var order = JsonSerializer.Deserialize<BookingTicketOrder>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (order is null)
                {
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        "DeserializationFailed",
                        "Kunde inte deserialiseras till BookingTicketOrder");
                    return;
                }

                var invoiceDto = new CreateInvoice
                {
                    CreatedDate = order.Created.DateTime,
                    DueDate = order.Created.AddDays(30).DateTime,
                    BookingId = order.Id.ToString(),
                    OriginalTicketId = order.TicketId,
                    Title = order.EventName,
                    Category = order.TicketCategory,
                    Price = (int)order.TicketPrice,
                    Qty = (int)order.TicketQuantity,
                    Amount = order.TotalTicketPrice * order.TicketQuantity,
                    Customer = new CustomerDto
                    {
                        CustomerName = order.UserName,
                        Address = order.UserStreetAddress,
                        PostalCode = order.UserPostalCode,
                        City = order.UserCity,
                        Email = order.UserEmail,
                        Phone = order.UserPhone
                    },
                    Event = new EventDto
                    {
                        EventName = order.EventName,
                        Address = order.EventStreetAddress,
                        PostalCode = order.EventPostalCode,
                        City = order.EventCity,
                        Email = order.EventEmail,
                        Phone = order.EventPhone
                    }
                };

                var created = await invoiceServices.CreateInvoiceAsync(invoiceDto);
                if (!created)
                    throw new Exception("CreateInvoiceAsync returnerade false.");

                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation("Meddelande {MessageId} färdigbehandlat.", args.Message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid bearbetning av meddelande {MessageId}", args.Message.MessageId);
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task OnErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(
                args.Exception,
                "Service Bus-fel: {ErrorSource}, {ErrorMessage}",
                args.ErrorSource,
                args.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
