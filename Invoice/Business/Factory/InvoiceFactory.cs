using Invoice.Business.Models;
using Invoice.Data.Entities;

namespace Invoice.Business.Factory;

public class InvoiceFactory
{
    public static Models.Invoice ToModel(InvoiceEntity entity)
    {
        return entity == null
            ? null!
            : new Models.Invoice
            {
                Id = entity.Id,
                InvoiceNumber = entity.InvoiceNumber,
                CreatedDate = entity.CreatedDate,
                DueDate = entity.DueDate,
                Paid = entity.Paid,
                TicketId = entity.TicketId,
                OriginalTicketId = entity.Ticket.OriginalTicketId,
                Title = entity.Ticket.Title,
                Category = entity.Ticket.Category,
                Price = entity.Ticket.Price,
                Qty = entity.Ticket.Qty,
                Amount = entity.Ticket.Amount,

                CustomerId = entity.CustomerId,
                CustomerName = entity.Customer.CustomerName,
                CustomerAddress = entity.Customer.Address,
                CustomerPostalCode = entity.Customer.PostalCode,
                CustomerCity = entity.Customer.City,
                CustomerEmail = entity.Customer.Email,
                CustomerPhone = entity.Customer.Phone,

                EventId = entity.EventId,
                EventName = entity.Event.Event,
                EventAddress = entity.Event.Address,
                EventPostalCode = entity.Event.PostalCode,
                EventCity = entity.Event.City,
                EventEmail = entity.Event.Email,
                EventPhone = entity.Event.Phone,
            };
    }
    public static InvoiceEntity ToEntity(CreateInvoice data)
    {
        if (data == null) return null;

        var ticket = new TicketSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            OriginalTicketId = data.OriginalTicketId,
            Title = data.Title,
            Category= data.Category,
            Price = data.Price,
            Qty = data.Qty,
            Amount = data.Price * data.Qty,
            CreatedDate = data.CreatedDate
        };
        var customer = new CustomerSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = data.Customer.CustomerName,
            Address = data.Customer.Address,
            PostalCode = data.Customer.PostalCode,
            City = data.Customer.City,
            Email = data.Customer.Email,
            Phone = data.Customer.Phone
        };

        var eventEntity = new EventSnapshot
        {
            Id = Guid.NewGuid().ToString(),
            Event = data.Event.EventName,
            Address = data.Event.Address,
            PostalCode = data.Event.PostalCode,
            City = data.Event.City,
            Email = data.Event.Email,
            Phone = data.Event.Phone
        };


        return new InvoiceEntity
        {
            Id = Guid.NewGuid().ToString(),
            InvoiceNumber = $"INV{new Random().Next(10000, 100000)}",
            CreatedDate = data.CreatedDate,
            DueDate = data.CreatedDate.AddDays(20),
            Paid = false,
            TicketId = ticket.Id,
            Ticket = ticket,
            CustomerId = customer.Id,
            Customer = customer,
            EventId = eventEntity.Id,
            Event = eventEntity
        };
    }
}
