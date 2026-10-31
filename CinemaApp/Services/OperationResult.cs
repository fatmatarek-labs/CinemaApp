namespace CinemaDashboard.Services;

public record OperationResult(bool Succeeded, string? Error = null, bool NotFound = false)
{
    public static OperationResult Ok() => new(true);
    public static OperationResult Fail(string error) => new(false, error);
    public static OperationResult Missing() => new(false, NotFound: true);
}