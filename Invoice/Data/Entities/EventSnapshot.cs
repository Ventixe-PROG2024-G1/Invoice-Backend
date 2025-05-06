using System.ComponentModel.DataAnnotations;

namespace Invoice.Data.Entities;

public class EventSnapshot
{
        [Key]
    public string Id { get; set; } = new Guid().ToString();
    public string Event { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

}
