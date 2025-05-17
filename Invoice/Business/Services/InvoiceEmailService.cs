using Invoice.Business.Builder;
using Invoice.Data.Repository;
using EmailServiceProvider.Services;
using Grpc.Core;
using Invoice.Business.Factory;


namespace Invoice.Business.Services;
public interface IInvoiceEmailService
{
    Task<bool> CreateInvoiceEmailAsync(string invoiceId);

}
public class InvoiceEmailService (ILogger<InvoiceEmailService> logger, EmailContract.EmailContractClient emailClient, IInvoiceRepository invoiceRepository, IInvoiceHtmlBuilder htmlBuilder, IInvoicePlainTextBuilder plainBuilder) : IInvoiceEmailService
{
    private readonly ILogger<InvoiceEmailService> _logger = logger;
    private readonly EmailContract.EmailContractClient _emailClient = emailClient;
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IInvoiceHtmlBuilder _htmlBuilder = htmlBuilder;
    private readonly IInvoicePlainTextBuilder _plainBuilder = plainBuilder;



    public async Task<bool> CreateInvoiceEmailAsync(string invoiceId)
    {
        var entity = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (entity == null)
            return false;
        var invoice = InvoiceFactory.ToModel(entity);

        var grpcRequest = new EmailMessageRequest
        {
            Recipients = { invoice.CustomerEmail },
            Subject = $"Faktura #{invoice.InvoiceNumber}",
            PlainText = _plainBuilder.Build(invoice),
            Html = _htmlBuilder.Build(invoice)
        };
        try
        {
            var response = await _emailClient.SendEmailAsync(grpcRequest);
            if (response.Succeeded)
            {
                _logger.LogInformation("Email sent for invoice {InvoiceId}.", invoiceId);
                return true;
            }
            _logger.LogError("gRPC EmailService returned error: {Error}", response.Error);
            return false;
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC call failed for invoice {InvoiceId}.", invoiceId);
            return false;
        }

    }
}

