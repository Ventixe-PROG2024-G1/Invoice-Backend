using System.ComponentModel.DataAnnotations;

namespace Invoice.Data.Entities;

public class TicketSnapshot
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OriginalTicketId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Price {  get; set; }
    public int Qty { get; set; }
    public decimal Amount {  get; set; }
    public DateTime CreatedDate { get; set; }

}
