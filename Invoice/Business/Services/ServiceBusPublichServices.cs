using Azure.Messaging.ServiceBus;
using Invoice.Business.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

public interface IServiceBusPublishService
{
    Task SendAsync(string invoiceId, string bookingId);
}
public class ServiceBusPublishService : IAsyncDisposable, IServiceBusPublishService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBusPublishService(IOptions<InvoiceServiceBusSettings> options)
    {

        _client = new ServiceBusClient(options.Value.ConnectionString);
        _sender = _client.CreateSender(options.Value.QueueName);
    }

    public async Task SendAsync(string invoiceId, string bookingId)
    {
        var IDs = new
        {
            InvoiceId = invoiceId,
            BookingId = bookingId
        };
        var json = JsonSerializer.Serialize(IDs);

        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString()
        };
        await _sender.SendMessageAsync(message);
    }
    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}