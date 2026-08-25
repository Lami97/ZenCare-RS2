using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZenCare.Model.Enums;
using ZenCare.Services.Database;
using ZenCare.Services.Interfaces;
using ZenCare.Services.Security;

namespace ZenCare.Services.Services;

public class EvaluatorDataSeeder : IEvaluatorDataSeeder
{
    private const string DemoPassword = "Demo123!";
    private static readonly DateTime SeedCreatedAt = new(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime RollingSlotSeedCreatedAt = SeedCreatedAt.AddMinutes(1);
    private static readonly IReadOnlyList<TimeSpan> RollingSlotStartTimes =
    [
        new(9, 0, 0),
        new(10, 0, 0),
        new(11, 0, 0),
        new(12, 0, 0),
        new(13, 0, 0),
        new(14, 0, 0),
        new(15, 0, 0),
        new(16, 0, 0)
    ];

    private readonly ZenCareDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EvaluatorDataSeeder> _logger;
    private readonly TimeZoneInfo _businessTimeZone;

    public EvaluatorDataSeeder(
        ZenCareDbContext dbContext,
        IConfiguration configuration,
        ILogger<EvaluatorDataSeeder> logger,
        TimeZoneInfo businessTimeZone)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _businessTimeZone = businessTimeZone;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            _logger.LogInformation("Evaluator demo-data seeding is disabled.");
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var adminRole = await RequireRoleAsync(UserRoleType.Admin, cancellationToken);
        var clientRole = await RequireRoleAsync(UserRoleType.Client, cancellationToken);
        var employeeRole = await RequireRoleAsync(UserRoleType.Employee, cancellationToken);

        var admin = await EnsureUserAsync(
            "evaluator.admin", "admin.demo@zencare.local", "Amira", "Kovacevic", "Demo", "Administrator", "061000001", cancellationToken);
        var client = await EnsureUserAsync(
            "evaluator.client", "client.demo@zencare.local", "Nina", "Maric", "Demo", "Client", "061000002", cancellationToken);
        var secondClient = await EnsureUserAsync(
            "evaluator.client2", "client2.demo@zencare.local", "Mina", "Selimovic", "Mina", "Client", "061000003", cancellationToken);
        var employeeUser = await EnsureUserAsync(
            "evaluator.employee", "employee.demo@zencare.local", "Lejla", "Karic", "Demo", "Therapist", "061000004", cancellationToken);
        var secondEmployeeUser = await EnsureUserAsync(
            "evaluator.employee2", "employee2.demo@zencare.local", "Sara", "Hadzic", "Sara", "Therapist", "061000005", cancellationToken);

        await EnsureRoleAssignmentAsync(admin, adminRole, cancellationToken);
        await EnsureRoleAssignmentAsync(client, clientRole, cancellationToken);
        await EnsureRoleAssignmentAsync(secondClient, clientRole, cancellationToken);
        await EnsureRoleAssignmentAsync(employeeUser, employeeRole, cancellationToken);
        await EnsureRoleAssignmentAsync(secondEmployeeUser, employeeRole, cancellationToken);

        await EnsureClientProfileAsync(client, new DateTime(1994, 4, 12, 0, 0, 0, DateTimeKind.Utc),
            "Female", "No known allergies.", "Relaxation and skin-care treatments.", cancellationToken);
        await EnsureClientProfileAsync(secondClient, new DateTime(1989, 9, 23, 0, 0, 0, DateTimeKind.Utc),
            "Female", null, "Aromatherapy and natural wellness products.", cancellationToken);

        var employee = await EnsureEmployeeAsync(
            employeeUser, "Massage therapy", "Licensed massage therapist focused on relaxation treatments.",
            new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc), cancellationToken);
        var secondEmployee = await EnsureEmployeeAsync(
            secondEmployeeUser, "Facial care and aromatherapy", "Wellness therapist specializing in facial care and aromatherapy.",
            new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc), cancellationToken);

        var massage = await EnsureServiceAsync(
            "Demo Relaxing Massage", "Relaxing Massage", "A calming full-body massage for stress relief.", 42.00m, 60, 1, cancellationToken);
        var facial = await EnsureServiceAsync(
            "Demo Facial Treatment", "Facial Treatment", "A restorative facial treatment using gentle wellness products.", 48.00m, 50, 3, cancellationToken);
        var aromatherapy = await EnsureServiceAsync(
            "Demo Aromatherapy Session", "Aromatherapy Session", "A guided aromatherapy session with selected essential oils.", 36.00m, 45, 2, cancellationToken);

        await EnsureEmployeeServiceAsync(employee, massage, cancellationToken);
        await EnsureEmployeeServiceAsync(employee, facial, cancellationToken);
        await EnsureEmployeeServiceAsync(secondEmployee, facial, cancellationToken);
        await EnsureEmployeeServiceAsync(secondEmployee, aromatherapy, cancellationToken);

        var naturalis = await EnsureSupplierAsync(
            "Naturalis Demo", "Naturalis", "orders@naturalis.example", "orders@naturalis-demo.local",
            "061100001", "Mostar, Bosnia and Herzegovina", cancellationToken);
        var botanica = await EnsureSupplierAsync(
            "Botanica Demo", "Botanica", "orders@botanica.example", "orders@botanica-demo.local",
            "061100002", "Sarajevo, Bosnia and Herzegovina", cancellationToken);

        var oil = await EnsureProductAsync(
            "DEMO-OIL-001", "Lavender Massage Oil", "Demo Lavender Massage Oil", "Lavender massage oil for relaxation and aromatherapy.",
            18.50m, 25, 1, 2, 2, naturalis.Id, cancellationToken);
        var cream = await EnsureProductAsync(
            "DEMO-CREAM-001", "Rose Face Cream", "Demo Rose Face Cream", "Hydrating rose face cream for daily skin care.",
            24.90m, 18, 2, 3, 2, botanica.Id, cancellationToken);
        var scrub = await EnsureProductAsync(
            "DEMO-SCRUB-001", "Natural Body Scrub", "Demo Natural Body Scrub", "A gentle body scrub made with natural ingredients.",
            16.75m, 20, 3, 4, 3, botanica.Id, cancellationToken);
        var giftSet = await EnsureProductAsync(
            "DEMO-GIFT-001", "Relaxation Gift Set", "Demo Relaxation Gift Set", "A wellness gift set containing relaxation essentials.",
            49.99m, 10, 5, 5, 4, naturalis.Id, cancellationToken);

        await EnsureCartAsync(client, cancellationToken);
        await EnsureCartAsync(secondClient, cancellationToken);

        var completedMassage = await EnsureAppointmentAsync(
            client, employee, massage, UtcDate(2026, 6, 10), new TimeSpan(9, 0, 0),
            AppointmentStatus.Completed, "Completed relaxation massage appointment.",
            "Demo completed massage appointment.", null, cancellationToken);
        await EnsureAppointmentAsync(
            client, secondEmployee, facial, UtcDate(2026, 7, 8), new TimeSpan(11, 0, 0),
            AppointmentStatus.Completed, "Completed facial care appointment.",
            "Demo completed facial appointment.", null, cancellationToken);
        await EnsureAppointmentAsync(
            client, secondEmployee, aromatherapy, UtcDate(2026, 7, 22), new TimeSpan(14, 0, 0),
            AppointmentStatus.Cancelled, "Cancelled aromatherapy appointment.",
            "Demo cancelled aromatherapy appointment.",
            "Client cancelled due to a schedule conflict.", cancellationToken);
        var completedAromatherapy = await EnsureAppointmentAsync(
            secondClient, secondEmployee, aromatherapy, UtcDate(2026, 6, 24), new TimeSpan(10, 0, 0),
            AppointmentStatus.Completed, "Completed aromatherapy appointment.",
            "Demo completed aromatherapy appointment.", null, cancellationToken);
        await EnsureAppointmentAsync(
            secondClient, employee, massage, UtcDate(2026, 7, 29), new TimeSpan(15, 0, 0),
            AppointmentStatus.NoShow, "Client did not attend the massage appointment.",
            "Demo no-show massage appointment.", null, cancellationToken);

        var businessToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _businessTimeZone).Date;
        await EnsureTimeSlotAsync(
            employee, massage, businessToday.AddDays(2), new TimeSpan(9, 0, 0), true, cancellationToken);
        await EnsureTimeSlotAsync(
            employee, facial, businessToday.AddDays(3), new TimeSpan(11, 0, 0), true, cancellationToken);
        await EnsureTimeSlotAsync(
            secondEmployee, aromatherapy, businessToday.AddDays(2), new TimeSpan(10, 0, 0), true, cancellationToken);
        var bookedSlot = await EnsureTimeSlotAsync(
            secondEmployee, facial, businessToday.AddDays(4), new TimeSpan(13, 0, 0), true, cancellationToken);
        await EnsureScheduledAppointmentAsync(
            secondClient, bookedSlot, "Upcoming evaluator schedule reservation.", cancellationToken);
        await EnsureTimeSlotAsync(
            employee, massage, businessToday.AddDays(5), new TimeSpan(15, 0, 0), false, cancellationToken);

        var rollingSlotCount = await EnsureRollingTimeSlotsAsync(
            employee, [massage, facial], businessToday, cancellationToken);
        rollingSlotCount += await EnsureRollingTimeSlotsAsync(
            secondEmployee, [facial, aromatherapy], businessToday, cancellationToken);

        _logger.LogInformation(
            "Added {RollingSlotCount} rolling evaluator schedule entries for the next six business days.",
            rollingSlotCount);

        var clientPurchase = await EnsurePurchaseAsync(
            client, "DEMO-PC-001", UtcDate(2026, 6, 5).AddHours(9), UtcDate(2026, 6, 5).AddHours(9).AddMinutes(8),
            PurchaseStatus.Completed, PaymentStatus.Succeeded,
            new[] { new SeedPurchaseItem(oil, 2), new SeedPurchaseItem(cream, 1) }, cancellationToken);
        await EnsurePurchaseAsync(
            client, "DEMO-PC-002", UtcDate(2026, 7, 2).AddHours(13), UtcDate(2026, 7, 2).AddHours(13).AddMinutes(6),
            PurchaseStatus.Completed, PaymentStatus.Succeeded,
            new[] { new SeedPurchaseItem(giftSet, 1) }, cancellationToken);
        await EnsurePurchaseAsync(
            secondClient, "DEMO-PC-003", UtcDate(2026, 6, 18).AddHours(12), UtcDate(2026, 6, 18).AddHours(12).AddMinutes(5),
            PurchaseStatus.Completed, PaymentStatus.Succeeded,
            new[] { new SeedPurchaseItem(scrub, 3), new SeedPurchaseItem(oil, 1) }, cancellationToken);
        await EnsurePurchaseAsync(
            secondClient, "DEMO-PC-004", UtcDate(2026, 7, 18).AddHours(16), null,
            PurchaseStatus.Cancelled, PaymentStatus.Cancelled,
            new[] { new SeedPurchaseItem(cream, 1) }, cancellationToken);

        await EnsureReviewAsync(client, completedMassage.Id, null, 5,
            "A relaxing and professional treatment.", cancellationToken);
        await EnsureReviewAsync(client, null, oil.Id, 5,
            "The lavender oil is excellent for relaxing evenings.", cancellationToken);
        await EnsureReviewAsync(client, null, cream.Id, 4,
            "The cream feels light and keeps the skin hydrated.", cancellationToken);
        await EnsureReviewAsync(secondClient, completedAromatherapy.Id, null, 5,
            "A very calming aromatherapy session.", cancellationToken);
        await EnsureReviewAsync(secondClient, null, scrub.Id, 5,
            "A gentle scrub with a pleasant natural scent.", cancellationToken);

        await EnsureProductViewAsync(client, oil, 5, UtcDate(2026, 7, 30).AddHours(18), cancellationToken);
        await EnsureProductViewAsync(client, giftSet, 3, UtcDate(2026, 7, 31).AddHours(19), cancellationToken);
        await EnsureProductViewAsync(secondClient, scrub, 6, UtcDate(2026, 7, 30).AddHours(17), cancellationToken);
        await EnsureProductViewAsync(secondClient, cream, 2, UtcDate(2026, 7, 28).AddHours(20), cancellationToken);

        await EnsureFaqAsync(1, "How can I book a wellness appointment?",
            "Open Services, choose a treatment and select an available schedule entry.",
            "Open Services, choose a treatment, select an available employee and confirm the date and time.", 1, cancellationToken);
        await EnsureFaqAsync(2, "How are product payments processed?",
            "Product payments are completed securely through Stripe.",
            "Product payments are completed securely through the Stripe test payment flow.", 2, cancellationToken);
        await EnsureFaqAsync(3, "Can I review a purchased product?",
            "Yes. A product can be reviewed after a completed and successfully paid purchase.",
            null, 3, cancellationToken);
        await EnsureFaqAsync(1, "How can I cancel an appointment?",
            "Open the appointment details and use the cancellation option while the appointment is still eligible for cancellation.",
            null, 4, cancellationToken);
        await EnsureFaqAsync(1, "Can I reschedule an existing appointment?",
            "Cancel the existing appointment and book another available schedule entry.",
            null, 5, cancellationToken);
        await EnsureFaqAsync(1, "Why is an appointment time unavailable?",
            "The schedule entry may already be booked, inactive or no longer in the future.",
            null, 6, cancellationToken);
        await EnsureFaqAsync(1, "Where can I see my appointment status?",
            "Open Appointments and select an appointment to view its current status and details.",
            null, 7, cancellationToken);
        await EnsureFaqAsync(2, "What happens if a payment is not completed?",
            "The purchase remains unpaid and can be paid again while it is still eligible for payment.",
            null, 8, cancellationToken);
        await EnsureFaqAsync(2, "Can the same purchase be paid more than once?",
            "No. ZenCare prevents duplicate payment confirmation for an already paid purchase.",
            null, 9, cancellationToken);
        await EnsureFaqAsync(2, "How are refunds processed?",
            "Eligible paid purchases are refunded through the Stripe test payment workflow.",
            null, 10, cancellationToken);
        await EnsureFaqAsync(2, "Where can I check my purchase status?",
            "Open Purchases and select a purchase to view its payment and fulfillment status.",
            null, 11, cancellationToken);
        await EnsureFaqAsync(3, "How can I browse wellness products?",
            "Open Products to browse the catalog and view product details.",
            null, 12, cancellationToken);
        await EnsureFaqAsync(3, "How are products recommended to me?",
            "Recommendations use your activity and preferences to suggest relevant products with an explanation.",
            null, 13, cancellationToken);
        await EnsureFaqAsync(3, "When can I review a product?",
            "A product can be reviewed after it belongs to a completed purchase with a successful payment.",
            null, 14, cancellationToken);
        await EnsureFaqAsync(3, "What happens when a product is out of stock?",
            "A quantity that exceeds the available stock cannot be added to a valid checkout.",
            null, 15, cancellationToken);
        await EnsureFaqAsync(4, "How can I find a wellness service?",
            "Open Services and use search or category filters to find a suitable treatment.",
            null, 16, cancellationToken);
        await EnsureFaqAsync(4, "Where can I see a service duration?",
            "The service details show its duration, price, category and description.",
            null, 17, cancellationToken);
        await EnsureFaqAsync(4, "How do I choose an employee for a service?",
            "Choose an available schedule entry; each entry identifies the employee assigned to that service and time.",
            null, 18, cancellationToken);
        await EnsureFaqAsync(5, "How can I create a client account?",
            "Use Create account on the sign-in screen and complete the registration form.",
            null, 19, cancellationToken);
        await EnsureFaqAsync(5, "How can I update my profile?",
            "Open Profile and choose Edit profile to update your personal information.",
            null, 20, cancellationToken);
        await EnsureFaqAsync(5, "What should I do if I forget my password?",
            "Use Forgot password on the sign-in screen and follow the reset instructions sent to your email address.",
            null, 21, cancellationToken);

        await EnsureNotificationAsync(client, "Welcome to ZenCare", "Welcome to the ZenCare demo",
            "Your account is ready for service booking, purchases and reviews.",
            "Your evaluator client account is ready for service booking, purchases and reviews.", true, cancellationToken);
        await EnsureNotificationAsync(client, "Appointment history available", "Demo appointment history available",
            "Your account includes completed and cancelled appointments.",
            "Your account includes completed and cancelled appointments for evaluator testing.", false, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation(
            "Evaluator demo data is ready. Seeded scenario includes {UserCount} users and purchase {PurchaseNumber}.",
            5, clientPurchase.PurchaseNumber);
    }

    private bool IsEnabled()
    {
        var configuredValue = _configuration["EvaluatorSeed:Enabled"];
        if (string.IsNullOrWhiteSpace(configuredValue))
        {
            return true;
        }

        if (bool.TryParse(configuredValue, out var enabled))
        {
            return enabled;
        }

        _logger.LogWarning("EvaluatorSeed:Enabled is invalid. Evaluator demo-data seeding will not run.");
        return false;
    }

    private async Task<Role> RequireRoleAsync(UserRoleType roleType, CancellationToken cancellationToken)
    {
        return await _dbContext.Roles.SingleAsync(role => role.RoleType == roleType, cancellationToken);
    }

    private async Task<User> EnsureUserAsync(
        string username,
        string email,
        string firstName,
        string lastName,
        string legacyFirstName,
        string legacyLastName,
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Username == username || candidate.Email == email, cancellationToken);

        if (user != null)
        {
            var changed = false;
            if (user.FirstName == legacyFirstName)
            {
                user.FirstName = firstName;
                changed = true;
            }

            if (user.LastName == legacyLastName)
            {
                user.LastName = lastName;
                changed = true;
            }

            if (changed)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return user;
        }

        var salt = PasswordHasher.GenerateSalt();
        user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            PhoneNumber = phoneNumber,
            PasswordSalt = salt,
            PasswordHash = PasswordHasher.GenerateHash(DemoPassword, salt),
            IsActive = true,
            CreatedAt = SeedCreatedAt
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    private async Task EnsureRoleAssignmentAsync(User user, Role role, CancellationToken cancellationToken)
    {
        if (await _dbContext.UserRoles.AnyAsync(
                userRole => userRole.UserId == user.Id && userRole.RoleId == role.Id, cancellationToken))
        {
            return;
        }

        _dbContext.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreatedAt = SeedCreatedAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureClientProfileAsync(
        User user,
        DateTime dateOfBirth,
        string gender,
        string? healthNotes,
        string preferences,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.ClientProfiles.AnyAsync(profile => profile.UserId == user.Id, cancellationToken))
        {
            return;
        }

        _dbContext.ClientProfiles.Add(new ClientProfile
        {
            UserId = user.Id,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            HealthNotes = healthNotes,
            Preferences = preferences,
            CreatedAt = SeedCreatedAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Employee> EnsureEmployeeAsync(
        User user,
        string specialization,
        string bio,
        DateTime hireDate,
        CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(item => item.UserId == user.Id, cancellationToken);
        if (employee != null)
        {
            return employee;
        }

        employee = new Employee
        {
            UserId = user.Id,
            Specialization = specialization,
            Bio = bio,
            HireDate = hireDate,
            IsAvailable = true,
            CreatedAt = SeedCreatedAt
        };
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    private async Task<WellnessService> EnsureServiceAsync(
        string legacyName,
        string name,
        string description,
        decimal price,
        int durationMinutes,
        int categoryId,
        CancellationToken cancellationToken)
    {
        var service = await _dbContext.WellnessServices
            .FirstOrDefaultAsync(item => item.Name == legacyName, cancellationToken);
        if (service != null)
        {
            var polishedNameExists = await _dbContext.WellnessServices
                .AnyAsync(item => item.Id != service.Id && item.Name == name, cancellationToken);
            if (!polishedNameExists)
            {
                service.Name = name;
                service.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning(
                    "Evaluator service {LegacyName} was not renamed because {Name} already exists.",
                    legacyName,
                    name);
            }

            return service;
        }

        service = await _dbContext.WellnessServices.FirstOrDefaultAsync(item => item.Name == name, cancellationToken);
        if (service != null)
        {
            return service;
        }

        service = new WellnessService
        {
            Name = name,
            Description = description,
            Price = price,
            DurationMinutes = durationMinutes,
            Status = ServiceStatus.Active,
            ServiceCategoryId = categoryId,
            CreatedAt = SeedCreatedAt
        };
        _dbContext.WellnessServices.Add(service);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return service;
    }

    private async Task EnsureEmployeeServiceAsync(
        Employee employee,
        WellnessService service,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.EmployeeServices.AnyAsync(
                item => item.EmployeeId == employee.Id && item.WellnessServiceId == service.Id, cancellationToken))
        {
            return;
        }

        _dbContext.EmployeeServices.Add(new Database.EmployeeService
        {
            EmployeeId = employee.Id,
            WellnessServiceId = service.Id,
            IsActive = true,
            CreatedAt = SeedCreatedAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Supplier> EnsureSupplierAsync(
        string legacyName,
        string name,
        string email,
        string legacyEmail,
        string phoneNumber,
        string address,
        CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(item => item.Name == legacyName, cancellationToken);
        if (supplier != null)
        {
            var changed = false;
            var polishedNameExists = await _dbContext.Suppliers
                .AnyAsync(item => item.Id != supplier.Id && item.Name == name, cancellationToken);
            if (!polishedNameExists)
            {
                supplier.Name = name;
                changed = true;
            }
            else
            {
                _logger.LogWarning(
                    "Evaluator supplier {LegacyName} was not renamed because {Name} already exists.",
                    legacyName,
                    name);
            }

            if (supplier.ContactEmail == legacyEmail)
            {
                supplier.ContactEmail = email;
                changed = true;
            }

            if (changed)
            {
                supplier.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return supplier;
        }

        supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(item => item.Name == name, cancellationToken);
        if (supplier != null)
        {
            return supplier;
        }

        supplier = new Supplier
        {
            Name = name,
            ContactEmail = email,
            PhoneNumber = phoneNumber,
            Address = address,
            IsActive = true,
            CreatedAt = SeedCreatedAt
        };
        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return supplier;
    }

    private async Task<Product> EnsureProductAsync(
        string sku,
        string name,
        string legacyName,
        string description,
        decimal price,
        int stockQuantity,
        int categoryId,
        int typeId,
        int unitId,
        int supplierId,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.SKU == sku, cancellationToken);
        if (product != null)
        {
            if (product.Name == legacyName)
            {
                product.Name = name;
                product.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return product;
        }

        product = new Product
        {
            SKU = sku,
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            Status = ProductStatus.Active,
            ProductCategoryId = categoryId,
            ProductTypeId = typeId,
            UnitOfMeasureId = unitId,
            SupplierId = supplierId,
            CreatedAt = SeedCreatedAt
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return product;
    }

    private async Task EnsureCartAsync(User user, CancellationToken cancellationToken)
    {
        if (await _dbContext.Carts.AnyAsync(cart => cart.UserId == user.Id, cancellationToken))
        {
            return;
        }

        _dbContext.Carts.Add(new Cart { UserId = user.Id, CreatedAt = SeedCreatedAt });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Appointment> EnsureAppointmentAsync(
        User user,
        Employee employee,
        WellnessService service,
        DateTime appointmentDate,
        TimeSpan startTime,
        AppointmentStatus status,
        string notes,
        string legacyNotes,
        string? cancellationReason,
        CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(item =>
            item.UserId == user.Id &&
            item.EmployeeId == employee.Id &&
            item.WellnessServiceId == service.Id &&
            item.AppointmentDate == appointmentDate &&
            item.StartTime == startTime,
            cancellationToken);

        if (appointment != null)
        {
            if (appointment.Notes == legacyNotes)
            {
                appointment.Notes = notes;
                appointment.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return appointment;
        }

        appointment = new Appointment
        {
            UserId = user.Id,
            EmployeeId = employee.Id,
            WellnessServiceId = service.Id,
            AppointmentDate = appointmentDate,
            StartTime = startTime,
            EndTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes)),
            Status = status,
            Notes = notes,
            CancellationReason = cancellationReason,
            CreatedAt = appointmentDate.AddDays(-7)
        };
        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return appointment;
    }

    private async Task<TimeSlot> EnsureTimeSlotAsync(
        Employee employee,
        WellnessService service,
        DateTime slotDate,
        TimeSpan startTime,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var slot = await _dbContext.TimeSlots
            .Include(item => item.Appointments)
            .FirstOrDefaultAsync(item =>
                item.EmployeeId == employee.Id &&
                item.WellnessServiceId == service.Id &&
                item.StartTime == startTime &&
                item.CreatedAt == SeedCreatedAt,
                cancellationToken);

        if (slot != null)
        {
            if (slot.Appointments.Count == 0)
            {
                slot.SlotDate = slotDate.Date;
                slot.EndTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                slot.IsActive = isActive;
                slot.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return slot;
        }

        slot = new TimeSlot
        {
            EmployeeId = employee.Id,
            WellnessServiceId = service.Id,
            SlotDate = slotDate.Date,
            StartTime = startTime,
            EndTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes)),
            IsActive = isActive,
            CreatedAt = SeedCreatedAt
        };

        _dbContext.TimeSlots.Add(slot);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return slot;
    }

    private async Task<int> EnsureRollingTimeSlotsAsync(
        Employee employee,
        IReadOnlyList<WellnessService> services,
        DateTime businessToday,
        CancellationToken cancellationToken)
    {
        if (!employee.IsAvailable || services.Count == 0)
        {
            return 0;
        }

        var employeeUserIsActive = await _dbContext.Users
            .Where(user => user.Id == employee.UserId)
            .Select(user => user.IsActive)
            .SingleAsync(cancellationToken);
        if (!employeeUserIsActive)
        {
            return 0;
        }

        var serviceIds = services.Select(service => service.Id).ToArray();
        var activeAssignedServiceIds = (await _dbContext.EmployeeServices
                .Where(assignment =>
                    assignment.EmployeeId == employee.Id &&
                    assignment.IsActive &&
                    serviceIds.Contains(assignment.WellnessServiceId))
                .Select(assignment => assignment.WellnessServiceId)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        var businessDays = GetNextBusinessDays(businessToday, 6);
        var firstDate = businessDays[0];
        var lastDate = businessDays[^1];

        var existingSlots = await _dbContext.TimeSlots
            .Where(slot =>
                slot.EmployeeId == employee.Id &&
                slot.SlotDate.Date >= firstDate &&
                slot.SlotDate.Date <= lastDate)
            .ToListAsync(cancellationToken);
        var existingAppointments = await _dbContext.Appointments
            .Where(appointment =>
                appointment.EmployeeId == employee.Id &&
                appointment.AppointmentDate.Date >= firstDate &&
                appointment.AppointmentDate.Date <= lastDate)
            .Select(appointment => new
            {
                Date = appointment.AppointmentDate.Date,
                appointment.StartTime,
                appointment.EndTime
            })
            .ToListAsync(cancellationToken);

        var addedCount = 0;
        for (var dayIndex = 0; dayIndex < businessDays.Count; dayIndex++)
        {
            var service = services[dayIndex % services.Count];
            if (service.Status != ServiceStatus.Active || !activeAssignedServiceIds.Contains(service.Id))
            {
                continue;
            }

            var slotDate = businessDays[dayIndex];
            foreach (var startTime in RollingSlotStartTimes)
            {
                var endTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                var exactSlotExists = existingSlots.Any(slot =>
                    slot.WellnessServiceId == service.Id &&
                    slot.SlotDate.Date == slotDate &&
                    slot.StartTime == startTime);
                if (exactSlotExists)
                {
                    continue;
                }

                var overlapsSlot = existingSlots.Any(slot =>
                    slot.SlotDate.Date == slotDate &&
                    startTime < slot.EndTime &&
                    endTime > slot.StartTime);
                var overlapsAppointment = existingAppointments.Any(appointment =>
                    appointment.Date == slotDate &&
                    startTime < appointment.EndTime &&
                    endTime > appointment.StartTime);
                if (overlapsSlot || overlapsAppointment)
                {
                    continue;
                }

                var slot = new TimeSlot
                {
                    EmployeeId = employee.Id,
                    WellnessServiceId = service.Id,
                    SlotDate = slotDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = true,
                    CreatedAt = RollingSlotSeedCreatedAt
                };

                _dbContext.TimeSlots.Add(slot);
                existingSlots.Add(slot);
                addedCount++;
            }
        }

        if (addedCount > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return addedCount;
    }

    private static IReadOnlyList<DateTime> GetNextBusinessDays(DateTime businessToday, int count)
    {
        var businessDays = new List<DateTime>(count);
        var date = businessToday.Date;

        while (businessDays.Count < count)
        {
            date = date.AddDays(1);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            businessDays.Add(date);
        }

        return businessDays;
    }

    private async Task EnsureScheduledAppointmentAsync(
        User user,
        TimeSlot slot,
        string notes,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.Appointments.AnyAsync(
                appointment => appointment.TimeSlotId == slot.Id,
                cancellationToken))
        {
            return;
        }

        _dbContext.Appointments.Add(new Appointment
        {
            UserId = user.Id,
            EmployeeId = slot.EmployeeId,
            WellnessServiceId = slot.WellnessServiceId,
            TimeSlotId = slot.Id,
            AppointmentDate = slot.SlotDate,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Status = AppointmentStatus.Confirmed,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Purchase> EnsurePurchaseAsync(
        User user,
        string purchaseNumber,
        DateTime createdAt,
        DateTime? paidAt,
        PurchaseStatus status,
        PaymentStatus paymentStatus,
        IReadOnlyCollection<SeedPurchaseItem> items,
        CancellationToken cancellationToken)
    {
        var purchase = await _dbContext.Purchases
            .FirstOrDefaultAsync(item => item.PurchaseNumber == purchaseNumber, cancellationToken);
        if (purchase != null)
        {
            return purchase;
        }

        purchase = new Purchase
        {
            UserId = user.Id,
            PurchaseNumber = purchaseNumber,
            Status = status,
            PaymentStatus = paymentStatus,
            TotalAmount = items.Sum(item => item.Product.Price * item.Quantity),
            StripePaymentIntentId = null,
            PaidAt = paidAt,
            CreatedAt = createdAt,
            UpdatedAt = status == PurchaseStatus.Completed ? paidAt : createdAt
        };
        _dbContext.Purchases.Add(purchase);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _dbContext.PurchaseItems.AddRange(items.Select(item => new PurchaseItem
        {
            PurchaseId = purchase.Id,
            ProductId = item.Product.Id,
            Quantity = item.Quantity,
            UnitPrice = item.Product.Price,
            TotalPrice = item.Product.Price * item.Quantity
        }));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return purchase;
    }

    private async Task EnsureReviewAsync(
        User user,
        int? appointmentId,
        int? productId,
        int rating,
        string comment,
        CancellationToken cancellationToken)
    {
        var exists = appointmentId.HasValue
            ? await _dbContext.Reviews.AnyAsync(
                review => review.UserId == user.Id && review.AppointmentId == appointmentId, cancellationToken)
            : await _dbContext.Reviews.AnyAsync(
                review => review.UserId == user.Id && review.ProductId == productId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.Reviews.Add(new Review
        {
            UserId = user.Id,
            AppointmentId = appointmentId,
            ProductId = productId,
            Rating = rating,
            Comment = comment,
            Status = ReviewStatus.Approved,
            CreatedAt = SeedCreatedAt.AddMonths(7)
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureProductViewAsync(
        User user,
        Product product,
        int viewCount,
        DateTime lastViewedAt,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.ProductViews.AnyAsync(
                view => view.UserId == user.Id && view.ProductId == product.Id, cancellationToken))
        {
            return;
        }

        _dbContext.ProductViews.Add(new ProductView
        {
            UserId = user.Id,
            ProductId = product.Id,
            ViewCount = viewCount,
            LastViewedAt = lastViewedAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureFaqAsync(
        int categoryId,
        string question,
        string answer,
        string? legacyAnswer,
        int displayOrder,
        CancellationToken cancellationToken)
    {
        var faq = await _dbContext.FAQs.FirstOrDefaultAsync(item => item.Question == question, cancellationToken);
        if (faq != null)
        {
            if (legacyAnswer != null && faq.Answer == legacyAnswer)
            {
                faq.Answer = answer;
                faq.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        _dbContext.FAQs.Add(new FAQ
        {
            FAQCategoryId = categoryId,
            Question = question,
            Answer = answer,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = SeedCreatedAt
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureNotificationAsync(
        User user,
        string title,
        string legacyTitle,
        string message,
        string legacyMessage,
        bool isRead,
        CancellationToken cancellationToken)
    {
        var notification = await _dbContext.Notifications.FirstOrDefaultAsync(
            item => item.UserId == user.Id && item.Title == legacyTitle, cancellationToken);
        if (notification != null)
        {
            var changed = false;
            var polishedTitleExists = await _dbContext.Notifications.AnyAsync(
                item => item.Id != notification.Id && item.UserId == user.Id && item.Title == title,
                cancellationToken);
            if (!polishedTitleExists)
            {
                notification.Title = title;
                changed = true;
            }

            if (notification.Message == legacyMessage)
            {
                notification.Message = message;
                changed = true;
            }

            if (notification.NotificationType == "Demo")
            {
                notification.NotificationType = "System";
                changed = true;
            }

            if (changed)
            {
                notification.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        notification = await _dbContext.Notifications.FirstOrDefaultAsync(
            item => item.UserId == user.Id && item.Title == title, cancellationToken);
        if (notification != null)
        {
            return;
        }

        _dbContext.Notifications.Add(new Notification
        {
            UserId = user.Id,
            Title = title,
            Message = message,
            NotificationType = "System",
            Status = NotificationStatus.Sent,
            IsRead = isRead,
            SentAt = SeedCreatedAt.AddMonths(7),
            CreatedAt = SeedCreatedAt.AddMonths(7)
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static DateTime UtcDate(int year, int month, int day)
    {
        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
    }

    private sealed record SeedPurchaseItem(Product Product, int Quantity);
}
