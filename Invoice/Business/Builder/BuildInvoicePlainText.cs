using System.Text;

namespace Invoice.Business.Builder;

public interface IInvoicePlainTextBuilder
{
    string Build(Models.Invoice invoice);
}
public class InvoicePlainTextBuilder : IInvoicePlainTextBuilder
{
    public string Build(Models.Invoice inv)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html><body>");
        sb.AppendLine($"<h1>Faktura #{inv.InvoiceNumber}</h1>");
        sb.AppendLine($"<p>Datum: {inv.CreatedDate:yyyy-MM-dd}</p>");

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
