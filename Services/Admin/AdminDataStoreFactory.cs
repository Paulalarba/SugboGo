using Microsoft.Extensions.Options;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Admin;

public sealed class AdminDataStoreFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly SupabaseOptions _options;

    public AdminDataStoreFactory(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SupabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _options = options.Value;
    }

    public IAdminDataStore Create()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            return _serviceProvider.GetRequiredService<PostgresAdminDataStore>();
        }

        if (!string.IsNullOrWhiteSpace(_options.Url) && !string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return _serviceProvider.GetRequiredService<SupabaseAdminDataStore>();
        }

        return _serviceProvider.GetRequiredService<LocalJsonAdminDataStore>();
    }
}
