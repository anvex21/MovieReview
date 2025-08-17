using System.ComponentModel.DataAnnotations;

namespace MovieReview.Models.DTOs
{
    public class RegisterDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        [Required, EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
