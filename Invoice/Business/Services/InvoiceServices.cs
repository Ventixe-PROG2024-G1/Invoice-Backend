using Invoice.Business.Factory;
using Invoice.Business.Models;
using Invoice.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace Invoice.Business.Services;
public interface IInvoiceServise
{
    Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync();
    Task<Models.Invoice?> GetInvoiceByIdAsync(string id);
    Task<bool> CreateInvoiceAsync(CreateInvoice form);
}
public class InvoiceServices(IInvoiceRepository invoiceRepository, IMemoryCache cache) : IInvoiceServise
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IMemoryCache _cache= cache;


    public async Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync()
    {
        var entities = await _invoiceRepository.GetAllAsync();  
        return entities
            .Select(InvoiceFactory.ToModel)
            .OrderByDescending(i => i.CreatedDate)
            .ToList();
    }
    public async Task<Models.Invoice?> GetInvoiceByIdAsync(string id)
    {
        var cacheKey = $"invoice_{id}";
        if (_cache.TryGetValue(cacheKey, out Models.Invoice cached))
            return cached;

        var entity = await _invoiceRepository.GetByIdAsync(id);
        if (entity == null)
            return null;

        var model = InvoiceFactory.ToModel(entity);

        var cacheData = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(1))
        .SetSize(1);

        _cache.Set(cacheKey, model, cacheData);

        return model;
    }
    public async Task<bool> CreateInvoiceAsync(CreateInvoice form )
    {
        if (form == null)
            return false;

        var entity = InvoiceFactory.ToEntity(form);
        return await _invoiceRepository.AddAsync(entity);
    }
}

