using Invoice.Business.Factory;
using Invoice.Business.Models;
using Invoice.Data.Entities;
using Invoice.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Intrinsics.Arm;

namespace Invoice.Business.Services;
public interface IInvoiceServices
{
    Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync();
    Task<Models.Invoice?> GetInvoiceByIdAsync(string id);
    Task<InvoiceEntity?> CreateInvoiceAsync(CreateInvoice form);
}
public class InvoiceServices(IInvoiceRepository invoiceRepository, IMemoryCache cache) : IInvoiceServices
{
    private const string AllInvoicesCacheKey = "all_invoices";
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IMemoryCache _cache= cache;

    public async Task<IEnumerable<Models.Invoice>> GetAllInvoicesAsync()
    {
        if (_cache.TryGetValue(AllInvoicesCacheKey, out List<Models.Invoice> cachedInvoices))
        {
            return cachedInvoices;
        }

        var entities = await _invoiceRepository.GetAllAsync();
        var invoices = entities
            .Select(e => InvoiceFactory.ToModel(e))
            .OrderByDescending(i => i.CreatedDate)
            .ToList();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetSize(invoices.Count);   

        _cache.Set(AllInvoicesCacheKey, invoices, cacheOptions);

        return invoices;
    }
    public async Task<Models.Invoice?> GetInvoiceByIdAsync(string id)
    {
        if (_cache.TryGetValue(AllInvoicesCacheKey, out List<Models.Invoice> allInvoices))
        {
            var fromCache = allInvoices.FirstOrDefault(inv => inv.Id == id);
            if (fromCache != null)
                return fromCache;
        }

        var entity = await _invoiceRepository.GetByIdAsync(id);
        if (entity == null)
            return null;

        var model = InvoiceFactory.ToModel(entity);

        return model;
    }
    public async Task<InvoiceEntity?> CreateInvoiceAsync(CreateInvoice form)
    {
        if (form == null)
            return null;

        var entity = InvoiceFactory.ToEntity(form);
        var success = await _invoiceRepository.AddAsync(entity);
        if (!success)
            return null;

        return entity;

    }

}

