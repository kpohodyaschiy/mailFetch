namespace YandexMailApi.Services;

public class CredentialStore
{
    private readonly object _lock = new();
    private string? _login;
    private string? _password;

    public void SetCredentials(string login, string password)
    {
        lock (_lock)
        {
            _login = login;
            _password = password;
        }
    }

    public (string? login, string? password) GetCredentials()
    {
        lock (_lock)
        {
            return (_login, _password);
        }
    }
}
