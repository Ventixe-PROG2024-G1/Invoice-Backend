using Invoice.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice.Data.Entities;

public class InvoiceEntity
{
    public string Id { get; set; }= new Guid().ToString();
    public string InvoiceNumber { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool Paid { get; set; }
    public string BookingId { get; set; } = null!;

    [ForeignKey(nameof(Ticket))]
    public string TicketId { get; set; } = null!;
    public virtual TicketSnapshot Ticket {  get; set; } = null!;

    [ForeignKey(nameof(Customer))]
    public string CustomerId { get; set; } = null!;
    public virtual CustomerSnapshot Customer{ get; set; } = null!;

    [ForeignKey(nameof(Event))]
    public string EventId { get; set; } = null!;
    public virtual EventSnapshot Event { get; set; } = null!;

}
