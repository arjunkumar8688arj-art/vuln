using BillingApi.Data;
using BillingApi.Reports;
using BillingApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<InvoiceRepository>();
builder.Services.AddSingleton<InvoiceCalculator>();
builder.Services.AddSingleton<ApiKeyHasher>();
builder.Services.AddSingleton<WebhookProcessor>();
builder.Services.AddSingleton<CsvInvoiceExporter>();
builder.Services.AddHttpClient<PaymentGatewayClient>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();
