namespace PermissionBasedAuth.Helpers;

public class JsonResponse
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; }
    public string ErrorMessage { get; set; }
}
