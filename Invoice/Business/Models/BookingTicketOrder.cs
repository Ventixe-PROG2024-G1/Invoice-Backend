using System.Text.Json.Serialization;
using System.Text.Json;

namespace Invoice.Business.Models;

public record BookingTicketOrder
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public int TicketQuantity { get; set; }
    public decimal TotalTicketPrice { get; set; }
    public decimal TicketPrice { get; set; }
    public string TicketCategory { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string UserPhone { get; set; } = null!;
    public string UserStreetAddress { get; set; } = null!;
    public string UserPostalCode { get; set; } = null!;
    public string UserCity { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string EventCategory { get; set; } = null!;
    public string EventEmail { get; set; } = null!;
    public string EventPhone { get; set; } = null!;
    public string EventStreetAddress { get; set; } = null!;
    public string EventPostalCode { get; set; } = null!;
    public string EventCity { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string EventId { get; set; } = null!;
    public string TicketId { get; set; } = null!;
}

