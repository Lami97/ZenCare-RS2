namespace ZenCare.Services.Interfaces;

public interface IEvaluatorDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
