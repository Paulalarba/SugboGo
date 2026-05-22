// program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SugboGo.Data;
using SugboGo.Services.Admin;
using SugboGo.Services.Auth;
using SugboGo.Services.BookingOptions;
using SugboGo.Services.Dashboard;
using SugboGo.Services.Travel;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SugboGoDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            npgsqlOptions.CommandTimeout(60);
        }));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")))
    .SetApplicationName("SogboGo");
builder.Services.Configure<SupabaseOptions>(builder.Configuration.GetSection("Supabase"));
builder.Services.Configure<AccountRoleOptions>(builder.Configuration.GetSection("Authentication"));
builder.Services.AddSingleton<IPasswordService, Pbkdf2PasswordService>();
builder.Services.AddSingleton<IAccountRoleService, AccountRoleService>();
builder.Services.AddScoped<LocalJsonUserAccountStore>();
builder.Services.AddScoped<PostgresUserAccountStore>();
builder.Services.AddHttpClient<SupabaseUserAccountStore>();
builder.Services.AddScoped<UserAccountStoreFactory>();
builder.Services.AddScoped<IUserAccountStore>(provider => provider.GetRequiredService<UserAccountStoreFactory>().Create());
builder.Services.AddScoped<LocalJsonTravelPreferenceStore>();
builder.Services.AddScoped<PostgresTravelPreferenceStore>();
builder.Services.AddHttpClient<SupabaseTravelPreferenceStore>();
builder.Services.AddScoped<TravelPreferenceStoreFactory>();
builder.Services.AddScoped<ITravelPreferenceStore>(provider => provider.GetRequiredService<TravelPreferenceStoreFactory>().Create());
builder.Services.AddScoped<ICebuRecommendationService, CebuRecommendationService>();
builder.Services.AddScoped<LocalJsonAdminDataStore>();
builder.Services.AddScoped<PostgresAdminDataStore>();
builder.Services.AddHttpClient<SupabaseAdminDataStore>();
builder.Services.AddScoped<AdminDataStoreFactory>();
builder.Services.AddScoped<IAdminDataStore>(provider => provider.GetRequiredService<AdminDataStoreFactory>().Create());
builder.Services.AddScoped<IAdminOperationsService, AdminOperationsService>();
builder.Services.AddScoped<IBookingOptionsService, BookingOptionsService>();

builder.Services.AddScoped<LocalJsonDestinationPostStore>();
builder.Services.AddScoped<PostgresDestinationPostStore>();
builder.Services.AddHttpClient<SupabaseDestinationPostStore>();
builder.Services.AddScoped<DestinationPostStoreFactory>();
builder.Services.AddScoped<IDestinationPostStore>(provider => provider.GetRequiredService<DestinationPostStoreFactory>().Create());

builder.Services.AddScoped<LocalJsonUserSavedGemStore>();
builder.Services.AddScoped<PostgresUserSavedGemStore>();
builder.Services.AddHttpClient<SupabaseUserSavedGemStore>();
builder.Services.AddScoped<UserSavedGemStoreFactory>();
builder.Services.AddScoped<IUserSavedGemStore>(provider => provider.GetRequiredService<UserSavedGemStoreFactory>().Create());

builder.Services.AddScoped<IDashboardExperienceService, DashboardExperienceService>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SogboGo.Auth";
        options.LoginPath = "/Account";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");

    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SugboGoDbContext>();
        await dbContext.Database.MigrateAsync();
        await TravelSpotSeeder.SeedAsync(dbContext);
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "Database migration or travel spot seeding failed. Data-backed travel features may be unavailable until migrations are applied.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Default route points to the SogboGo landing page.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
