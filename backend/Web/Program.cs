using ActivityGameBackend.Persistence.Mssql;
using ActivityGameBackend.Persistence.Mssql.ServiceCollectionExtensions;
using ActivityGameBackend.Web.Filters;
using ActivityGameBackend.Web.Mappings;
using ActivityGameBackend.Web.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var clientId = configuration["Authentication:Google:ClientId"];
var clientSecret = configuration["Authentication:Google:ClientSecret"];

if (clientId is null || clientSecret is null)
{
    throw new InvalidOperationException("Google ClientId and ClientSecret most be provided.");
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services
    .AddMsSqlAppDbContext(configuration, environment)
    .AddMsSqlDbServiceProvider();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        // options.SaveTokens = true;
        // options.Events = new OAuthEvents
        // {
        //     OnCreatingTicket = context =>
        //     {
        //         var accessToken = context.AccessToken;
        //         var refreshToken = context.RefreshToken;
        //         return Task.CompletedTask;
        //     },
        // };
    });

builder.Services
    .AddAutoMapper(typeof(DtoMappingProfiles))
    .AddAutoMapper(typeof(EntityToDomainMappingProfile))
    .AddMemoryCache()
    .ConfigureScrutor("ActivityGameBackend");

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRClientPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173") // Your frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services
    .AddHttpClient()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger().UseSwaggerUI();
    ApplyMigrationsAndSeedData(app);
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.MapHub<GameHub>("/hubs/game");

app.Run();

static void ApplyMigrationsAndSeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    DataSeeder.SeedWordsAsync(dbContext).GetAwaiter().GetResult();
}
