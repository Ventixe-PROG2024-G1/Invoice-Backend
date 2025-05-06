using System.ComponentModel.DataAnnotations;

namespace Invoice.Data.Entity;

public class TicketSnapshot
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OriginalTicketId {  get; set; }
    public string Title { get; set; }
    public int Price {  get; set; }
    public int Qty { get; set; }
    public decimal Amount {  get; set; }
    public DateTime CreatedDate { get; set; }

}
