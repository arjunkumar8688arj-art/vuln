using Microsoft.AspNetCore.Mvc;

namespace BillingApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [HttpGet("callback")]
    public IActionResult Callback([FromQuery] string? code, [FromQuery] string? returnUrl)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest("missing authorization code");

        // Session exchange happens in the gateway; here we only send the browser back where it came from.
        if (string.IsNullOrEmpty(returnUrl))
        {
            return Redirect("/");
        }
        return Redirect(returnUrl);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("billing_session");
        return LocalRedirect("/");
    }
}
