using SugboGo.Models;

namespace SugboGo.Services.Auth;

public interface IUserAccountStore
{
    Task<List<UserAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAccount> CreateAsync(UserAccount account, CancellationToken cancellationToken = default);
}
