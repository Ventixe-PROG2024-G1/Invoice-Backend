using Invoice.Data.Context;
using Invoice.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Invoice.Data.Repository
{
    public interface IInvoiceRepository
    {
        Task<bool> AddAsync(InvoiceEntity entity);
        Task<IEnumerable<InvoiceEntity>> GetAllAsync();
        Task<InvoiceEntity?> GetByIdAsync(string invoiceId);
        Task<IEnumerable<InvoiceEntity>> GetOverdueAsync();
    }
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<InvoiceEntity> _dbSet;

        public InvoiceRepository(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<InvoiceEntity>();
        }
        public async Task<bool> AddAsync(InvoiceEntity entity)
        {
            if (entity == null)
                return false;

            try
            {
                _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<IEnumerable<InvoiceEntity>> GetAllAsync()
        {
            return await _dbSet
                .Include(i => i.Ticket)
                .Include(i => i.Customer)
                .Include(i => i.Event)
                .ToListAsync();
        }

        public async Task<InvoiceEntity?> GetByIdAsync(string invoiceId)
        {
            return await _dbSet
                .Include(i => i.Ticket)
                .Include(i => i.Customer)
                .Include(i => i.Event)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        public async Task<IEnumerable<InvoiceEntity>> GetOverdueAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
            .Where(i => i.DueDate < today && !i.Paid)
            .OrderBy(i => i.DueDate)
            .ToListAsync();

        }
    }
}
