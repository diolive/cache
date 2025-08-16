namespace DioLive.Cache.WebUI.Models;

public class ErrorViewModel
{
    public required string RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}