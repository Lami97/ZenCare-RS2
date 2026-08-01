namespace ZenCare.Services.Interfaces;

public interface IBootstrapAdminService
{
    Task BootstrapAsync(CancellationToken cancellationToken = default);
}
