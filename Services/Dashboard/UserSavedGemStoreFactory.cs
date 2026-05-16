using Microsoft.Extensions.Options;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Dashboard;

public sealed class UserSavedGemStoreFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly SupabaseOptions _options;

    public UserSavedGemStoreFactory(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SupabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _options = options.Value;
    }

    public IUserSavedGemStore Create()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            return _serviceProvider.GetRequiredService<PostgresUserSavedGemStore>();
        }

        if (!string.IsNullOrWhiteSpace(_options.Url) && !string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return _serviceProvider.GetRequiredService<SupabaseUserSavedGemStore>();
        }

        return _serviceProvider.GetRequiredService<LocalJsonUserSavedGemStore>();
    }
}
