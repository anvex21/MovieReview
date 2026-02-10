namespace MovieReview.Models.DTOs
{
    public class AuthResultDto
    {
        /// <summary>
        /// Success or not
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// JWT
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public required string Message { get; set; }
    }
}
