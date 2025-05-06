using System.ComponentModel.DataAnnotations;

namespace Invoice.Business.Models;

public class Invoice
{
    [Required]
    public string Id { get; set; } = new Guid().ToString();
    [Required]
    public string InvoiceNumber { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    [Required]
    public bool Paid { get; set; }
    public string TicketId { get; set; }
    public string OriginalTicketId { get; set; }
    public string Title { get; set; }
    public int Price { get; set; }
    public int Qty { get; set; }
    public decimal Amount { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string EventId { get; set; }
    public string EventName { get; set; }

}
