using Azure.Messaging.ServiceBus;
using Invoice.Business.Builder;
using Invoice.Business.Factory;
using Invoice.Business.Models;
using Invoice.Data.Repository;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Invoice.Business.Services
{
    public interface IInvoiceEmailService
    {
        Task<bool> CreateInvoiceEmailAsync(string invoiceId);
    }

    public class ServiceBusEmailService(ILogger<ServiceBusEmailService> logger,ServiceBusClient serviceBusClient, ServiceBusEmailSettings settings,IInvoiceRepository invoiceRepository,IInvoiceHtmlBuilder htmlBuilder,IInvoicePlainTextBuilder plainBuilder) : IInvoiceEmailService
    {
        private readonly ILogger<ServiceBusEmailService> _logger = logger;
        private readonly ServiceBusEmailSettings _settings = settings;
        private readonly ServiceBusClient _serviceBusClient = serviceBusClient;
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IInvoiceHtmlBuilder _htmlBuilder = htmlBuilder;
        private readonly IInvoicePlainTextBuilder _plainBuilder = plainBuilder;

        public async Task<bool> CreateInvoiceEmailAsync(string invoiceId)
        {
            var entity = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (entity == null)
                return false;

            var invoice = InvoiceFactory.ToModel(entity);

            var emailMessage = new
            {
                Recipients = new[] { invoice.CustomerEmail },
                Subject = $"Faktura #{invoice.InvoiceNumber}",
                PlainText = _plainBuilder.Build(invoice),
                Html = _htmlBuilder.Build(invoice)
            };

            try
            {
                var messageBody = JsonSerializer.Serialize(emailMessage);

                ServiceBusSender sender = _serviceBusClient.CreateSender(_settings.QueueName);
                var message = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json"
                };

                await sender.SendMessageAsync(message);

                _logger.LogInformation("Email message sent to ServiceBus for invoice {InvoiceId}.", invoiceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ServiceBus send failed for invoice {InvoiceId}.", invoiceId);
                return false;
            }
        }
    }
}
