using BillingApi.Data;
using BillingApi.Reports;
using Microsoft.AspNetCore.Mvc;

namespace BillingApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly string _reportRoot;
    private readonly InvoiceRepository _repository;
    private readonly CsvInvoiceExporter _exporter;

    public ReportsController(IConfiguration configuration, InvoiceRepository repository, CsvInvoiceExporter exporter)
    {
        _reportRoot = configuration["Reports:OutputDirectory"] ?? Path.Combine(AppContext.BaseDirectory, "reports");
        _repository = repository;
        _exporter = exporter;
        Directory.CreateDirectory(_reportRoot);
    }

    [HttpPost("invoices/export")]
    public async Task<IActionResult> Export([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var invoices = await _repository.GetIssuedBetweenAsync(from, to);
        var rows = invoices.Select(i => new InvoiceRow
        {
            Number = i.Number,
            CustomerId = i.CustomerId,
            IssuedAt = i.IssuedAt,
            DueAt = i.DueAt,
            Status = i.Status
        });

        var fileName = $"invoices-{from:yyyyMMdd}-{to:yyyyMMdd}.csv";
        _exporter.Export(rows, Path.Combine(_reportRoot, fileName));
        return Ok(new { file = fileName });
    }

    [HttpGet("download")]
    public IActionResult Download([FromQuery] string file)
    {
        var fullPath = Path.Combine(_reportRoot, file);
        if (!System.IO.File.Exists(fullPath)) return NotFound();
        return PhysicalFile(fullPath, "text/csv", Path.GetFileName(fullPath));
    }
}
