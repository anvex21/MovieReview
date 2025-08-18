﻿namespace MovieReview.Models.DTOs
{
    public class ReviewDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Rating (FK)
        /// </summary>
        public int Rating { get; set; }
        /// <summary>
        /// MovieId (FK)
        /// </summary>
        public long MovieId { get; set; }
        /// <summary>
        /// UserId (FK)
        /// </summary>
        public long UserId { get; set; }
    }
}
