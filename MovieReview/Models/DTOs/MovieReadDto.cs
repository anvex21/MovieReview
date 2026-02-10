namespace MovieReview.Models.DTOs
{
    public class MovieReadDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Release year
        /// </summary>
        public int ReleaseYear { get; set; }
        /// <summary>
        /// Review count
        /// </summary>
        public int ReviewCount { get; set; }
        /// <summary>
        /// Average rating
        /// </summary>
        public double AverageRating { get; set; }
        /// <summary>
        /// IMDb rating (from OMDb), as a string (e.g. "8.5" or "N/A")
        /// </summary>
        public string? ImdbRating { get; set; }
    }
}
