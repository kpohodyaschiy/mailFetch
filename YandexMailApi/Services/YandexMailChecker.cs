using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace YandexMailApi.Services;

public class YandexMailChecker : BackgroundService
{
    private readonly ILogger<YandexMailChecker> _logger;
    private readonly CredentialStore _store;
    private readonly HashSet<UniqueId> _seen = new();

    public YandexMailChecker(ILogger<YandexMailChecker> logger, CredentialStore store)
    {
        _logger = logger;
        _store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckMailAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task CheckMailAsync(CancellationToken token)
    {
        var (login, password) = _store.GetCredentials();
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            _logger.LogInformation("Credentials not set. Skipping mail check.");
            return;
        }

        try
        {
            using var client = new ImapClient();
            await client.ConnectAsync("imap.yandex.com", 993, true, token);
            await client.AuthenticateAsync(login, password, token);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly, token);
            var query = SearchQuery.NotSeen;
            var uids = await inbox.SearchAsync(query, token);

            foreach (var uid in uids)
            {
                if (_seen.Contains(uid)) continue;
                var message = await inbox.GetMessageAsync(uid, token);
                if (message.From.Mailboxes.Any(m => m.Address.EndsWith("@reg.ru", StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogInformation("New mail from {From}: {Subject}", message.From, message.Subject);
                }
                _seen.Add(uid);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking mail");
        }
    }
}
