# YandexMailApi

This project is a minimal .NET 8 Web API that periodically checks a Yandex mailbox for new messages from `reg.ru` senders. Credentials can be provided via HTTP POST to `/api/credentials`.

## Usage
1. Build and run the project using the .NET 8 SDK:
   ```bash
   dotnet run --project YandexMailApi
   ```
2. POST login information as JSON:
   ```bash
   curl -X POST http://localhost:5000/api/credentials \
        -H "Content-Type: application/json" \
        -d '{"login": "your@yandex.com", "password": "secret"}'
   ```

The background service checks the inbox every 5 minutes and logs any new unread emails from senders at `reg.ru`.
