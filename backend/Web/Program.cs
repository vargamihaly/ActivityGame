using ActivityGameBackend.Persistence.Mssql;
using ActivityGameBackend.Persistence.Mssql.ServiceCollectionExtensions;
using ActivityGameBackend.Web.Filters;
using ActivityGameBackend.Web.Mappings;
using ActivityGameBackend.Web.ServiceCollectionExtensions;
using dotenv.net;
using dotenv.net.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
DotEnv.Load(options: new DotEnvOptions(
    envFilePaths: new[] { envFilePath },
    probeForEnv: true
));

var clientId = EnvReader.GetStringValue("GOOGLE_CLIENT_ID");
var clientSecret = EnvReader.GetStringValue("GOOGLE_CLIENT_SECRET");

if (clientId is null || clientSecret is null)
{
    throw new InvalidOperationException("Google ClientId and ClientSecret most be provided.");
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.local.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

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
            .WithOrigins("http://localhost:5173")
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
