namespace MovieReview.Models.DTOs;

public class ErrorResponse
{
    /// <summary>
    /// StatusCode
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Details
    /// </summary>
    public string? Details { get; set; } // Only in development
}