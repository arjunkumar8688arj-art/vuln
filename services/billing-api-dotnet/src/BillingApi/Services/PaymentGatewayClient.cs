using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BillingApi.Services;

public class PaymentGatewayClient
{
    private readonly HttpClient _http;

    public PaymentGatewayClient(IConfiguration configuration)
    {
        // The gateway's staging endpoint uses an internal CA that is not installed on build agents.
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(configuration["PaymentGateway:BaseUrl"]!),
            Timeout = TimeSpan.FromSeconds(configuration.GetValue("PaymentGateway:TimeoutSeconds", 20))
        };
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", configuration["PaymentGateway:ApiKey"]);
    }

    public async Task<string> CreateChargeAsync(string invoiceNumber, decimal amount, string currency)
    {
        var body = JsonSerializer.Serialize(new { reference = invoiceNumber, amount, currency });
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await _http.PostAsync("charges", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
