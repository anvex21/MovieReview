using Microsoft.AspNetCore.Identity;

namespace MovieReview.Models.Entities
{
    public class User : IdentityUser<long>
    {
        
        /// <summary>
        /// Navigation properties
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
