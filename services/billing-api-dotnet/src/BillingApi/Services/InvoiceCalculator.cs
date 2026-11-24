using BillingApi.Models;

namespace BillingApi.Services;

public class InvoiceCalculator
{
    private readonly decimal _defaultTaxRate;
    private readonly int _paymentTermsDays;

    public InvoiceCalculator(IConfiguration configuration)
    {
        _defaultTaxRate = configuration.GetValue("Billing:DefaultTaxRate", 0.18m);
        _paymentTermsDays = configuration.GetValue("Billing:PaymentTermsDays", 30);
    }

    public decimal Subtotal(Invoice invoice) => invoice.Lines.Sum(l => l.LineTotal);

    public decimal Tax(Invoice invoice) => Math.Round(Subtotal(invoice) * _defaultTaxRate, 2, MidpointRounding.AwayFromZero);

    public decimal Total(Invoice invoice) => Subtotal(invoice) + Tax(invoice);

    public DateTime DueDate(DateTime issuedAt) => issuedAt.Date.AddDays(_paymentTermsDays);

    public bool IsOverdue(Invoice invoice, DateTime now) =>
        invoice.Status != "Paid" && invoice.DueAt.Date < now.Date;
}
