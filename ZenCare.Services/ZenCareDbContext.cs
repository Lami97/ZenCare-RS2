using Microsoft.EntityFrameworkCore;
using ZenCare.Model.Enums;
using ZenCare.Services.Database;

namespace ZenCare.Services;

public class ZenCareDbContext : DbContext
{
    public ZenCareDbContext(DbContextOptions<ZenCareDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<BusinessReport> BusinessReports { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<ClientProfile> ClientProfiles { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeService> EmployeeServices { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<FAQCategory> FAQCategories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<RecommendationLog> RecommendationLogs { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<WellnessService> WellnessServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithOne(e => e.User)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.ClientProfile)
            .WithOne(cp => cp.User)
            .HasForeignKey<ClientProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        modelBuilder.Entity<EmployeeService>()
            .HasIndex(es => new { es.EmployeeId, es.WellnessServiceId })
            .IsUnique();

        modelBuilder.Entity<CartItem>()
            .HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Purchase>()
            .Property(p => p.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RecommendationLog>()
            .Property(rl => rl.Score)
            .HasPrecision(18, 4);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeService>()
            .HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeServices)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeService>()
            .HasOne(es => es.WellnessService)
            .WithMany(ws => ws.EmployeeServices)
            .HasForeignKey(es => es.WellnessServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WellnessService>()
            .HasOne(ws => ws.ServiceCategory)
            .WithMany(sc => sc.WellnessServices)
            .HasForeignKey(ws => ws.ServiceCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.WellnessService)
            .WithMany()
            .HasForeignKey(a => a.WellnessServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Appointment)
            .WithMany(a => a.Payments)
            .HasForeignKey(p => p.AppointmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Purchase)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.PurchaseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Appointment)
            .WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductType)
            .WithMany(pt => pt.Products)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.UnitOfMeasure)
            .WithMany(uom => uom.Products)
            .HasForeignKey(p => p.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Purchase>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Purchase)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.PurchaseItems)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecommendationLog>()
            .HasOne(rl => rl.User)
            .WithMany()
            .HasForeignKey(rl => rl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecommendationLog>()
            .HasOne(rl => rl.Product)
            .WithMany(p => p.RecommendationLogs)
            .HasForeignKey(rl => rl.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RecommendationLog>()
            .HasOne(rl => rl.WellnessService)
            .WithMany()
            .HasForeignKey(rl => rl.WellnessServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BusinessReport>()
            .HasOne(br => br.GeneratedByUser)
            .WithMany()
            .HasForeignKey(br => br.GeneratedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FAQ>()
            .HasOne(f => f.FAQCategory)
            .WithMany(fc => fc.FAQs)
            .HasForeignKey(f => f.FAQCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", RoleType = UserRoleType.Admin, Description = "Administrator role", IsActive = true, CreatedAt = createdAt },
            new Role { Id = 2, Name = "Employee", RoleType = UserRoleType.Employee, Description = "Employee role", IsActive = true, CreatedAt = createdAt },
            new Role { Id = 3, Name = "Client", RoleType = UserRoleType.Client, Description = "Client role", IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<ServiceCategory>().HasData(
            new ServiceCategory { Id = 1, Name = "Masaže", IsActive = true, CreatedAt = createdAt },
            new ServiceCategory { Id = 2, Name = "Aromaterapija", IsActive = true, CreatedAt = createdAt },
            new ServiceCategory { Id = 3, Name = "Njega lica", IsActive = true, CreatedAt = createdAt },
            new ServiceCategory { Id = 4, Name = "Wellness tretmani", IsActive = true, CreatedAt = createdAt },
            new ServiceCategory { Id = 5, Name = "Relaksacija", IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, Name = "Eterična ulja", IsActive = true, CreatedAt = createdAt },
            new ProductCategory { Id = 2, Name = "Njega kože", IsActive = true, CreatedAt = createdAt },
            new ProductCategory { Id = 3, Name = "Pilinzi", IsActive = true, CreatedAt = createdAt },
            new ProductCategory { Id = 4, Name = "Wellness preparati", IsActive = true, CreatedAt = createdAt },
            new ProductCategory { Id = 5, Name = "Poklon paketi", IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<ProductType>().HasData(
            new ProductType { Id = 1, Name = "Preparat", IsActive = true, CreatedAt = createdAt },
            new ProductType { Id = 2, Name = "Ulje", IsActive = true, CreatedAt = createdAt },
            new ProductType { Id = 3, Name = "Krema", IsActive = true, CreatedAt = createdAt },
            new ProductType { Id = 4, Name = "Piling", IsActive = true, CreatedAt = createdAt },
            new ProductType { Id = 5, Name = "Paket", IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<UnitOfMeasure>().HasData(
            new UnitOfMeasure { Id = 1, Name = "kom", Abbreviation = "kom", IsActive = true, CreatedAt = createdAt },
            new UnitOfMeasure { Id = 2, Name = "ml", Abbreviation = "ml", IsActive = true, CreatedAt = createdAt },
            new UnitOfMeasure { Id = 3, Name = "g", Abbreviation = "g", IsActive = true, CreatedAt = createdAt },
            new UnitOfMeasure { Id = 4, Name = "pakovanje", Abbreviation = "pakovanje", IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<FAQCategory>().HasData(
            new FAQCategory { Id = 1, Name = "Rezervacije", IsActive = true, CreatedAt = createdAt },
            new FAQCategory { Id = 2, Name = "Plaćanje", IsActive = true, CreatedAt = createdAt },
            new FAQCategory { Id = 3, Name = "Preparati", IsActive = true, CreatedAt = createdAt },
            new FAQCategory { Id = 4, Name = "Wellness usluge", IsActive = true, CreatedAt = createdAt },
            new FAQCategory { Id = 5, Name = "Korisnički račun", IsActive = true, CreatedAt = createdAt }
        );
    }
}
