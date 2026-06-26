namespace ZenCare.Model.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.")
    {
    }
}
