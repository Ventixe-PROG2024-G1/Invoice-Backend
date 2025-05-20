using System.Text;

namespace Invoice.Business.Builder
{
    public interface IInvoiceHtmlBuilder
    {
        string Build(Models.Invoice invoice);
    }

    public class InvoiceHtmlBuilder : IInvoiceHtmlBuilder
    {
        public string Build(Models.Invoice inv)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body>");
            sb.AppendLine(@"<div>");
            sb.AppendLine(@"  <div>");
            sb.AppendLine($"    <h3>#{inv.InvoiceNumber}</h3>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"  <div>");
            sb.AppendLine($"    <p>Issued Date {inv.CreatedDate:yyyy-MM-dd}</p>");
            sb.AppendLine($"    <p>Due Date {inv.DueDate:yyyy-MM-dd}</p>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"</div>");

            // Body
            sb.AppendLine(@"<div>");
            sb.AppendLine(@"  <div>");
            sb.AppendLine(@"    <p>Bill from:</p>");
            sb.AppendLine($"    <h4>{inv.EventName}</h4>");
            sb.AppendLine($"    <p>{inv.EventAddress}, {inv.EventPostalCode} {inv.EventCity}</p>");
            sb.AppendLine($"    <p>{inv.EventEmail}</p>");
            sb.AppendLine($"    <p>{inv.EventPhone}</p>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"  <div>");
            sb.AppendLine(@"    <p>Bill to:</p>");
            sb.AppendLine($"    <h4>{inv.CustomerName}</h4>");
            sb.AppendLine($"    <p>{inv.CustomerAddress}, {inv.CustomerPostalCode} {inv.CustomerCity}</p>");
            sb.AppendLine($"    <p>{inv.CustomerEmail}</p>");
            sb.AppendLine($"    <p>{inv.CustomerPhone}</p>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"</div>");

            sb.AppendLine("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\">");
            sb.AppendLine("  <thead>");
            sb.AppendLine("    <tr><th>Artikel</th><th>Antal</th><th>Enhetspris</th><th>Summa</th></tr>");
            sb.AppendLine("  </thead>");
            sb.AppendLine("  <tbody>");

            foreach (var item in inv.Items)
            {
                var lineTotal = item.Quantity * item.Price;
                sb.AppendLine($"    <tr>" +
                              $"<td>{item.Description}</td>" +
                              $"<td align=\"right\">{item.Quantity}</td>" +
                              $"<td align=\"right\">{item.Price:0.00} kr</td>" +
                              $"<td align=\"right\">{lineTotal:0.00} kr</td>" +
                              $"</tr>");
            }

            sb.AppendLine("  </tbody>");
            sb.AppendLine("</table>");

            sb.AppendLine($"<p><strong>Totalt: {inv.Amount:0.00} kr</strong></p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
    }
}
