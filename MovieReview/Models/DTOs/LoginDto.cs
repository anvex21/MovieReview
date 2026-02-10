using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.DTOs
{
    public class LoginDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public required string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public required string Password { get; set; }
    }
}
