using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Invoice.Business.Models
{
    public class Invoice
    {
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        [Required]
        public string InvoiceNumber { get; set; } = null!;

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public bool Paid { get; set; }
        public string BookingId { get; set; }
        public string? OriginalTicketId { get; set; }
        public string Category { get; set; } = null!;
        public string? Title { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }

        public decimal Amount { get; set; }

        //Customer
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string CustomerAddress { get; set; } = null!;
        public string CustomerPostalCode { get; set; } = null!;
        public string CustomerCity { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;

        //Event
        public string? EventId   { get; set; }
        public string? EventName { get; set; }
        public string EventAddress { get; set; } = null!;
        public string EventPostalCode { get; set; } = null!;
        public string EventCity { get; set; } = null!;
        public string EventEmail { get; set; } = null!;
        public string EventPhone { get; set; } = null!;

        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
