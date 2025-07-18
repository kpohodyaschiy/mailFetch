using Microsoft.AspNetCore.Mvc;
using YandexMailApi.Services;
using YandexMailApi.Models;

namespace YandexMailApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController : ControllerBase
{
    private readonly CredentialStore _store;

    public CredentialsController(CredentialStore store)
    {
        _store = store;
    }

    [HttpPost]
    public IActionResult SetCredentials(MailCredentials creds)
    {
        if (string.IsNullOrWhiteSpace(creds.Login) || string.IsNullOrWhiteSpace(creds.Password))
        {
            return BadRequest("Login and password required");
        }

        _store.SetCredentials(creds.Login, creds.Password);
        return Ok();
    }
}
