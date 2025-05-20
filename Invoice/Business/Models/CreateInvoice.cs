using System.ComponentModel.DataAnnotations;

namespace Invoice.Business.Models;

public class CreateInvoice
{
    [Required]
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    [Required]
    public string OriginalTicketId { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public int Price { get; set; }
    public int Qty { get; set; }
    public decimal Amount { get; set; }

    public CustomerDto Customer { get; set; }
    public EventDto Event { get; set; }
}

public class CustomerDto
{
    [Required]
    public string CustomerName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string City { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class EventDto
{
    [Required]
    public string EventName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string City { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

