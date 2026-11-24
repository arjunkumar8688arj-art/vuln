using System.Text.RegularExpressions;
using BillingApi.Data;
using BillingApi.Services;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace BillingApi.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(InvoicesController));
    private static readonly Regex InvoiceNumberPattern = new(@"^INV-\d{4}-\d{6}$", RegexOptions.Compiled);

    private readonly InvoiceRepository _repository;
    private readonly InvoiceCalculator _calculator;
    private readonly PaymentGatewayClient _gateway;

    public InvoicesController(InvoiceRepository repository, InvoiceCalculator calculator, PaymentGatewayClient gateway)
    {
        _repository = repository;
        _calculator = calculator;
        _gateway = gateway;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice is null) return NotFound();

        return Ok(new
        {
            invoice.Id,
            invoice.Number,
            invoice.CustomerId,
            invoice.Status,
            Subtotal = _calculator.Subtotal(invoice),
            Tax = _calculator.Tax(invoice),
            Total = _calculator.Total(invoice),
            Overdue = _calculator.IsOverdue(invoice, DateTime.UtcNow)
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string customer)
    {
        if (string.IsNullOrWhiteSpace(customer)) return BadRequest("customer is required");
        var results = await _repository.SearchByCustomerAsync(customer);
        return Ok(results);
    }

    [HttpPost("{id:int}/charge")]
    public async Task<IActionResult> Charge(int id)
    {
        var invoice = await _repository.GetByIdAsync(id);
        if (invoice is null) return NotFound();
        if (!InvoiceNumberPattern.IsMatch(invoice.Number))
        {
            Log.Warn($"Invoice {id} has a malformed number");
            return UnprocessableEntity("Invoice number is malformed");
        }

        var result = await _gateway.CreateChargeAsync(invoice.Number, _calculator.Total(invoice), "USD");
        return Ok(result);
    }
}
