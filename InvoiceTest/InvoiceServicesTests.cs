using Invoice.Business.Models;
using Invoice.Business.Services;
using Invoice.Data.Entities;
using Invoice.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using Moq;

public class InvoiceServicesTests
{
    private IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetAllInvoicesAsync_Returns_From_Repository_And_Caches()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var entities = new List<InvoiceEntity>
        {
            new InvoiceEntity { Id = "1", CreatedDate = DateTime.UtcNow },
            new InvoiceEntity { Id = "2", CreatedDate = DateTime.UtcNow.AddMinutes(-1) }
        };
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = (await service.GetAllInvoicesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("1", result[0].Id);
        Assert.Equal("2", result[1].Id);

        Assert.True(cache.TryGetValue("all_invoices", out List<Invoice.Business.Models.Invoice> cached));
        Assert.Equal(2, cached.Count);
    }

    [Fact]
    public async Task GetAllInvoicesAsync_Returns_From_Cache_If_Present()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var cachedInvoices = new List<Invoice.Business.Models.Invoice>
        {
            new Invoice.Business.Models.Invoice { Id = "3", CreatedDate = DateTime.UtcNow },
            new Invoice.Business.Models.Invoice { Id = "4", CreatedDate = DateTime.UtcNow.AddMinutes(-2) }
        };
        cache.Set("all_invoices", cachedInvoices);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = (await service.GetAllInvoicesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("3", result[0].Id);
        Assert.Equal("4", result[1].Id);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_Returns_From_Cache()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var cachedInvoices = new List<Invoice.Business.Models.Invoice>
        {
            new Invoice.Business.Models.Invoice { Id = "5" }
        };
        cache.Set("all_invoices", cachedInvoices);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = await service.GetInvoiceByIdAsync("5");
        Assert.NotNull(result);
        Assert.Equal("5", result.Id);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_Returns_From_Repository_If_Not_Cached()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var entity = new InvoiceEntity { Id = "6" };
        mockRepo.Setup(r => r.GetByIdAsync("6")).ReturnsAsync(entity);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = await service.GetInvoiceByIdAsync("6");
        Assert.NotNull(result);
        Assert.Equal("6", result.Id);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_Returns_Null_If_Not_Found()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        mockRepo.Setup(r => r.GetByIdAsync("nope")).ReturnsAsync((InvoiceEntity)null);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = await service.GetInvoiceByIdAsync("nope");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateInvoiceAsync_Returns_False_If_Form_Is_Null()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = await service.CreateInvoiceAsync(null);
        Assert.False(result);
    }

    [Fact]
    public async Task CreateInvoiceAsync_Sends_Bus_And_Adds_Invoice()
    {
        var mockRepo = new Mock<IInvoiceRepository>();
        var cache = CreateCache();
        var mockBus = new Mock<IServiceBusPublishService>();

        var form = new CreateInvoice { BookingId = "B1" };
        mockRepo.Setup(r => r.AddAsync(It.IsAny<InvoiceEntity>())).ReturnsAsync(true);

        var service = new InvoiceServices(mockRepo.Object, cache, mockBus.Object);

        var result = await service.CreateInvoiceAsync(form);

        Assert.True(result);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<InvoiceEntity>()), Times.Once);
        mockBus.Verify(b => b.SendAsync(It.IsAny<string>(), "B1"), Times.Once);
    }
}
