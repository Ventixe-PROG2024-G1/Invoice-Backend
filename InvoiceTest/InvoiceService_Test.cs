
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;
using Invoice.Business.Services;
using Invoice.Data.Repository;   
using Invoice.Data.Entities; 

namespace InvoiceTest
{
    public class InvoiceServices_GetAllInvoicesAsync_Tests
    {
        private readonly Mock<IInvoiceRepository> _repoMock;
        private readonly InvoiceServices _service;

        public InvoiceServices_GetAllInvoicesAsync_Tests()
        {
            _repoMock = new Mock<IInvoiceRepository>();
            // Vi behöver ingen cache för just denna test
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new InvoiceServices(_repoMock.Object, memoryCache);
        }

        [Fact]
        public async Task GetAllInvoicesAsync_KallarRepoEnGång_och_ReturnerarSamtligaMappedOchSorterade()
        {
            var older = new InvoiceEntity { Id = "A", CreatedDate = new DateTime(2021, 1, 1) };
            var newer = new InvoiceEntity { Id = "B", CreatedDate = new DateTime(2022, 1, 1) };
            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new[] { older, newer });
            
            var result = (await _service.GetAllInvoicesAsync()).ToList();
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);

            Assert.Equal(2, result.Count);

            Assert.Equal("B", result[0].Id);
            Assert.Equal("A", result[1].Id);
        }

        [Fact]
        public async Task GetAllInvoicesAsync_RepoReturnerarTomLista_ReturnerarTomLista()
        {
            _repoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(Array.Empty<InvoiceEntity>());

            var result = await _service.GetAllInvoicesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);

            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
