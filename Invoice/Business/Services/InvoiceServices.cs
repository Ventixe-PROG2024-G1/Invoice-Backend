using Invoice.Business.Factory;
using Invoice.Business.Models;
using Invoice.Data.Repository;

namespace Invoice.Business.Services;
public interface IInvoiceServise
{
    Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync();
    Task<Models.Invoice?> GetInvoiceByIdAsync(string id);
    Task<bool> CreateInvoiceAsync(CreateInvoice form);
}
public class InvoiceServices(IInvoiceRepository invoiceRepository) : IInvoiceServise
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;


    public async Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync()
    {
        var entities = await _invoiceRepository.GetAllAsync();  
        return entities
            .Select(InvoiceFactory.ToModel)
            .ToList();
    }
    public async Task<Models.Invoice?> GetInvoiceByIdAsync(string id)
    {
        var entity = await _invoiceRepository.GetByIdAsync(id);
        if (entity == null)
            return null;
        return InvoiceFactory.ToModel(entity);
    }
    public async Task<bool> CreateInvoiceAsync(CreateInvoice form )
    {
        if (form == null)
            return false;

        var entity = InvoiceFactory.ToEntity(form);
        return await _invoiceRepository.AddAsync(entity);
    }
}

