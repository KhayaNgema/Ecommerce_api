using Ecommerce_api.Data;
using Ecommerce_api.Helpers;
using Ecommerce_api.Interfaces;
using Ecommerce_api.Models;
using Ecommerce_api.Services;
using Ecommerce.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Localization;


var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog Configuration ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ---------- Connection String ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ---------- Services Configuration ----------
builder.Services.AddDbContext<Ecommerce_apiDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<UserBaseModel, IdentityRole>()
    .AddEntityFrameworkStores<Ecommerce_apiDBContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456; // 256 MB
});

// Encryption Configuration
var key = builder.Configuration["AES_KEY"];
var iv = builder.Configuration["AES_IV"];
var keyBytes = Convert.FromBase64String(key);
var ivBytes = Convert.FromBase64String(iv);
builder.Services.AddSingleton(new EncryptionConfiguration { Key = keyBytes, Iv = ivBytes });
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnyRole", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity.IsAuthenticated &&
            context.User.Claims.Any(c => c.Type == ClaimTypes.Role)));
});

// Identity Token Lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(72);
});

// JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

// Custom Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<RandomPasswordGeneratorService>();
builder.Services.AddScoped<RequestLogService>();
builder.Services.AddHttpClient<DeviceInfoService>();
builder.Services.AddHttpClient();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = false;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
        UsePageLocksOnDequeue = true,
        SchemaName = "hangfire"
    }));
builder.Services.AddHangfireServer();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(2001); 
    options.ListenLocalhost(7135, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});


// ---------- Build App ----------
var app = builder.Build();

// ✅ Swagger Enabled Always
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API V1");
    c.RoutePrefix = "swagger";
});


SetCulture("en-US");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Cookies.Append("StrictlyNecessaryCookie", "Value", new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
    });
    await next.Invoke();
});

var defaultCulture = new CultureInfo("en-US");

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
});

app.UseStaticFiles();

// Optional: Hangfire Dashboard
/*
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

RecurringJob.AddOrUpdate<FixtureService>(
    "schedule-fixtures",
    service => service.ScheduleFixturesAsync(),
    Cron.Weekly(DayOfWeek.Monday, 0, 0));

RecurringJob.AddOrUpdate<SubscriptionCheckerService>(
    "check-expired-subscriptions",
    service => service.CheckExpiredSubscriptions(),
    Cron.Minutely);

RecurringJob.AddOrUpdate<CompetitionService>(
    "end-monthly-competition",
    service => service.EndCurrentCompetitionAndStartNewOne(),
    "59 23 L * *");
*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tabRedirect",
    pattern: "{tab?}",
    defaults: new { controller = "Home", action = "Index" },
    constraints: new { tab = @"sportnews|fixtures|standings|matchresults|clubs|players|managers|topScores|topAssists" });

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.CreateRolesAndDefaultUser();
}

app.Run();

void SetCulture(string cultureCode)
{
    var culture = CultureInfo.CreateSpecificCulture(cultureCode);
    Thread.CurrentThread.CurrentCulture = culture;
    Thread.CurrentThread.CurrentUICulture = culture;
}
