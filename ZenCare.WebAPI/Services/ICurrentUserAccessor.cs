namespace ZenCare.WebAPI.Services;

public interface ICurrentUserAccessor
{
    int? GetUserId();
}
