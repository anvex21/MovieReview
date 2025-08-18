namespace MovieReview.Models.DTOs
{
    public class MovieQueryDto
    {
        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }   // filter by name
        /// <summary>
        /// Sort by
        /// </summary>
        public string? SortBy { get; set; } // e.g. "Name", "ReleaseDate"
        /// <summary>
        /// IsDescending
        /// </summary>
        public bool IsDescending { get; set; } = false;
        /// <summary>
        /// PageNumber
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
