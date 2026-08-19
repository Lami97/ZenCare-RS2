using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZenCare.Services;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Mapping;
using ZenCare.Services.Services;
using ZenCare.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ZenCareDbContext>(options =>
    options.UseSqlServer(connectionString));

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
builder.Services.AddScoped<IBootstrapAdminService, BootstrapAdminService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["JwtToken:Issuer"],
            ValidAudience = builder.Configuration["JwtToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"] ?? string.Empty)),
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

await ApplyDatabaseMigrationsAsync(app);
await BootstrapAdminAsync(app);

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task BootstrapAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var bootstrapAdminService = scope.ServiceProvider.GetRequiredService<IBootstrapAdminService>();
    await bootstrapAdminService.BootstrapAsync();
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
