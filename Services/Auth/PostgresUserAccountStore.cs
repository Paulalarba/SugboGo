using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Models;

namespace SugboGo.Services.Auth;

public sealed class PostgresUserAccountStore : IUserAccountStore
{
    private readonly SugboGoDbContext _dbContext;

    public PostgresUserAccountStore(SugboGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .OrderByDescending(user => user.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<UserAccount> CreateAsync(UserAccount account, CancellationToken cancellationToken = default)
    {
        account.Email = NormalizeEmail(account.Email);
        account.CreatedAt = DateTimeOffset.UtcNow;

        _dbContext.Users.Add(account);
        
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // PostgreSQL unique violation error code is usually handled by EF Core, 
            // but we can be specific if needed. 
            // For simplicity, we assume unique email constraint is violated.
            throw new InvalidOperationException("An account already exists for this email address.", ex);
        }

        return account;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
