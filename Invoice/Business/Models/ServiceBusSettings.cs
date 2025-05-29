namespace Invoice.Business.Models;

public class BookingServiceBusSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}

public class InvoiceServiceBusSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}

public class EmailServiceBusSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}
