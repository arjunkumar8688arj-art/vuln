namespace BillingApi.Models;

public record InvoiceLine(string Sku, string Description, int Quantity, decimal UnitPrice)
{
    public decimal LineTotal => Quantity * UnitPrice;
}

public class Invoice
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime DueAt { get; set; }
    public string Status { get; set; } = "Draft";
    public List<InvoiceLine> Lines { get; set; } = new();
}

public class WebhookEvent
{
    public string EventId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public object? Data { get; set; }
}
