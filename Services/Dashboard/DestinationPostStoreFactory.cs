using Microsoft.Extensions.Options;
using SugboGo.Services.Auth;

namespace SugboGo.Services.Dashboard;

public sealed class DestinationPostStoreFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly SupabaseOptions _options;

    public DestinationPostStoreFactory(IServiceProvider serviceProvider, IConfiguration configuration, IOptions<SupabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _options = options.Value;
    }

    public IDestinationPostStore Create()
    {
        if (!string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")))
        {
            return _serviceProvider.GetRequiredService<PostgresDestinationPostStore>();
        }

        if (!string.IsNullOrWhiteSpace(_options.Url) && !string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return _serviceProvider.GetRequiredService<SupabaseDestinationPostStore>();
        }

        return _serviceProvider.GetRequiredService<LocalJsonDestinationPostStore>();
    }
}
