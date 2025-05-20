namespace Invoice.Business.Models;

public class ServiceBusReceiveSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}
public class ServiceBusSendSettings
{
    public string ConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
}
