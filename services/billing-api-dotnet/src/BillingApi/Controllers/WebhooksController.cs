using BillingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingApi.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly WebhookProcessor _processor;

    public WebhooksController(WebhookProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost("payments")]
    public async Task<IActionResult> Payments()
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        var evt = _processor.Parse(payload);
        if (evt is null) return BadRequest("invalid payload");

        await _processor.HandleAsync(evt);
        return Accepted();
    }
}
