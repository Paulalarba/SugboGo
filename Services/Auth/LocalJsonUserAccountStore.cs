using System.Text.Json;
using SugboGo.Models;

namespace SugboGo.Services.Auth;

public sealed class LocalJsonUserAccountStore : IUserAccountStore
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public LocalJsonUserAccountStore(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "App_Data", "auth-users.json");
    }

    public async Task<List<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var users = await ReadUsersAsync(cancellationToken);
            users.ForEach(user => user.Role = AccountRoles.Normalize(user.Role));
            return users.OrderByDescending(user => user.CreatedAt).ToList();
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var users = await ReadUsersAsync(cancellationToken);
            var user = users.FirstOrDefault(user => NormalizeEmail(user.Email) == normalizedEmail);

            if (user is not null)
            {
                user.Role = AccountRoles.Normalize(user.Role);
            }

            return user;
        }
        finally
        {
            FileLock.Release();
        }
    }

    public async Task<UserAccount> CreateAsync(UserAccount account, CancellationToken cancellationToken = default)
    {
        account.Email = NormalizeEmail(account.Email);
        account.Role = AccountRoles.Normalize(account.Role);

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            var users = await ReadUsersAsync(cancellationToken);

            if (users.Any(user => NormalizeEmail(user.Email) == account.Email))
            {
                throw new InvalidOperationException("An account already exists for this email address.");
            }

            users.Add(account);
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, users, _jsonOptions, cancellationToken);

            return account;
        }
        finally
        {
            FileLock.Release();
        }
    }

    private async Task<List<UserAccount>> ReadUsersAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<UserAccount>>(stream, _jsonOptions, cancellationToken) ?? [];
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
