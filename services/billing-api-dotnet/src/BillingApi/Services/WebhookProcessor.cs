using BillingApi.Models;
using log4net;
using Newtonsoft.Json;

namespace BillingApi.Services;

public class WebhookProcessor
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(WebhookProcessor));

    public WebhookEvent? Parse(string payload)
    {
        // Partner payloads carry a polymorphic "Data" node, so type metadata is honoured.
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        return JsonConvert.DeserializeObject<WebhookEvent>(payload, settings);
    }

    public Task HandleAsync(WebhookEvent evt)
    {
        switch (evt.Type)
        {
            case "invoice.paid":
                Log.Info($"Invoice paid event {evt.EventId}");
                break;
            case "invoice.payment_failed":
                Log.Warn($"Payment failed event {evt.EventId}");
                break;
            default:
                Log.Debug($"Ignoring event type {evt.Type}");
                break;
        }
        return Task.CompletedTask;
    }
}
