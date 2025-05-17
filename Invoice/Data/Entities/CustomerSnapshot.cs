using System.ComponentModel.DataAnnotations;

namespace Invoice.Data.Entities;

public class CustomerSnapshot
{
    [Key]
    public string Id { get; set; } = new Guid().ToString();
    public string CustomerName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;

}
