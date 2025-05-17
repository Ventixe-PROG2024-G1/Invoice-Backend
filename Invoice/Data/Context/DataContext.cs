using Invoice.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Data.Context
{
    public class DataContext(DbContextOptions<DataContext> option) : DbContext(option)
    {
        public virtual DbSet<InvoiceEntity> Invoices { get; set; }
        public virtual DbSet<TicketSnapshot> TicketSnapshots { get; set; }
        public virtual DbSet<CustomerSnapshot> Customers { get; set; }
        public virtual DbSet<EventSnapshot> Events { get; set; }
    }

}
