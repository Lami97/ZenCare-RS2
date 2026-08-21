using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZenCare.Services;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Mapping;
using ZenCare.Services.Services;
using ZenCare.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
const string corsPolicyName = "ConfiguredOrigins";
const string developmentJwtIssuer = "ZenCare";
const string developmentJwtAudience = "ZenCareUsers";
const string developmentJwtDurationInMinutes = "60";

var developmentJwtFallbackApplied = ApplyDevelopmentJwtDefaults(
    builder.Configuration,
    builder.Environment,
    developmentJwtIssuer,
    developmentJwtAudience,
    developmentJwtDurationInMinutes);
var jwtSettings = GetValidatedJwtSettings(builder.Configuration);
var businessTimeZone = GetBusinessTimeZone(builder.Configuration, builder.Environment);

var allowedCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? Array.Empty<string>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ZenCareDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserAccessor, HttpCurrentUserAccessor>();
builder.Services.AddSingleton(businessTimeZone);

builder.Services.AddAutoMapper(_ => { }, typeof(RoleProfile).Assembly, typeof(ServiceCategoryProfile).Assembly, typeof(ProductCategoryProfile).Assembly, typeof(ProductTypeProfile).Assembly, typeof(UnitOfMeasureProfile).Assembly, typeof(FAQCategoryProfile).Assembly, typeof(FAQProfile).Assembly, typeof(ServiceProfile).Assembly, typeof(ProductProfile).Assembly, typeof(UserProfile).Assembly, typeof(UserRoleProfile).Assembly, typeof(ClientProfileProfile).Assembly, typeof(EmployeeProfile).Assembly, typeof(EmployeeServiceProfile).Assembly, typeof(AppointmentProfile).Assembly, typeof(PaymentProfile).Assembly, typeof(ReviewProfile).Assembly, typeof(PurchaseProfile).Assembly, typeof(PurchaseItemProfile).Assembly, typeof(CartProfile).Assembly, typeof(CartItemProfile).Assembly, typeof(NotificationProfile).Assembly, typeof(RecommendationLogProfile).Assembly, typeof(BusinessReportProfile).Assembly, typeof(SupplierProfile).Assembly);

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
builder.Services.AddScoped<IFAQCategoryService, FAQCategoryService>();
builder.Services.AddScoped<IFAQService, FAQService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IClientProfileService, ClientProfileService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeServiceService, EmployeeServiceService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPurchaseItemService, PurchaseItemService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationEventPublisher, NotificationEventPublisher>();
builder.Services.AddScoped<IRecommendationLogService, RecommendationLogService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IBusinessReportService, BusinessReportService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordResetEmailService, SmtpPasswordResetEmailService>();
builder.Services.AddScoped<IBootstrapAdminService, BootstrapAdminService>();
builder.Services.AddScoped<IEvaluatorDataSeeder, EvaluatorDataSeeder>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);

                if (string.IsNullOrWhiteSpace(jti))
                {
                    context.Fail("Token identifier is missing.");
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<ZenCareDbContext>();
                var isRevoked = await dbContext.RevokedTokens
                    .AnyAsync(rt => rt.Jti == jti && rt.ExpiresAt > DateTime.UtcNow);

                if (isRevoked)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        if (allowedCorsOrigins.Length > 0)
        {
            policy.WithOrigins(allowedCorsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token only.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddControllers();

var app = builder.Build();

if (developmentJwtFallbackApplied)
{
    app.Logger.LogWarning(
        "Development-only JWT fallback configuration is active. Configure JwtToken values through environment variables or User Secrets to override it.");
}

await ApplyDatabaseMigrationsAsync(app);
await BootstrapAdminAsync(app);
await SeedEvaluatorDataAsync(app);

// Configure the HTTP request pipeline.
var swaggerEnabled = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static TimeZoneInfo GetBusinessTimeZone(
    IConfiguration configuration,
    IHostEnvironment environment)
{
    var timeZoneId = configuration["BusinessTime:TimeZoneId"];

    if (string.IsNullOrWhiteSpace(timeZoneId))
    {
        if (environment.IsDevelopment())
        {
            return TimeZoneInfo.Local;
        }

        throw new InvalidOperationException(
            "Business time zone is not configured. Set BusinessTime__TimeZoneId.");
    }

    try
    {
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
    catch (TimeZoneNotFoundException ex)
    {
        throw new InvalidOperationException(
            $"Business time zone '{timeZoneId}' was not found on this system.", ex);
    }
    catch (InvalidTimeZoneException ex)
    {
        throw new InvalidOperationException(
            $"Business time zone '{timeZoneId}' is invalid.", ex);
    }
}

static bool ApplyDevelopmentJwtDefaults(
    ConfigurationManager configuration,
    IHostEnvironment environment,
    string issuer,
    string audience,
    string durationInMinutes)
{
    if (!environment.IsDevelopment())
    {
        return false;
    }

    var defaults = new Dictionary<string, string?>
    {
        ["JwtToken:Issuer"] = issuer,
        ["JwtToken:Audience"] = audience,
        ["JwtToken:SecretKey"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        ["JwtToken:DurationInMinutes"] = durationInMinutes
    };

    var missingDefaults = defaults
        .Where(setting => string.IsNullOrWhiteSpace(configuration[setting.Key]))
        .ToDictionary(setting => setting.Key, setting => setting.Value);

    if (missingDefaults.Count == 0)
    {
        return false;
    }

    configuration.AddInMemoryCollection(missingDefaults);
    return true;
}

static (string Issuer, string Audience, string SecretKey) GetValidatedJwtSettings(
    IConfiguration configuration)
{
    var issuer = configuration["JwtToken:Issuer"];
    var audience = configuration["JwtToken:Audience"];
    var secretKey = configuration["JwtToken:SecretKey"];
    var duration = configuration["JwtToken:DurationInMinutes"];

    var missingSettings = new[]
    {
        (Name: "JwtToken:Issuer", Value: issuer),
        (Name: "JwtToken:Audience", Value: audience),
        (Name: "JwtToken:SecretKey", Value: secretKey),
        (Name: "JwtToken:DurationInMinutes", Value: duration)
    }
    .Where(setting => string.IsNullOrWhiteSpace(setting.Value))
    .Select(setting => setting.Name)
    .ToArray();

    if (missingSettings.Length > 0)
    {
        throw new InvalidOperationException(
            $"JWT configuration is incomplete. Configure: {string.Join(", ", missingSettings)}.");
    }

    if (!int.TryParse(duration, out var durationInMinutes) || durationInMinutes <= 0)
    {
        throw new InvalidOperationException(
            "JWT configuration is invalid. JwtToken:DurationInMinutes must be a positive integer.");
    }

    if (Encoding.UTF8.GetByteCount(secretKey!) < 32)
    {
        throw new InvalidOperationException(
            "JWT configuration is invalid. JwtToken:SecretKey must contain at least 32 UTF-8 bytes.");
    }

    return (issuer!, audience!, secretKey!);
}

static async Task BootstrapAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var bootstrapAdminService = scope.ServiceProvider.GetRequiredService<IBootstrapAdminService>();
    await bootstrapAdminService.BootstrapAsync();
}

static async Task SeedEvaluatorDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var evaluatorDataSeeder = scope.ServiceProvider.GetRequiredService<IEvaluatorDataSeeder>();
    await evaluatorDataSeeder.SeedAsync();
}

static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
{
    const int maxAttempts = 12;
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseMigration");

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ZenCareDbContext>();
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in 5 seconds.", attempt, maxAttempts);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    using var finalScope = app.Services.CreateScope();
    var finalDbContext = finalScope.ServiceProvider.GetRequiredService<ZenCareDbContext>();
    await finalDbContext.Database.MigrateAsync();
}
