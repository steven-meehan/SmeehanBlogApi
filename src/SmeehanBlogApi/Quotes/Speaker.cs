namespace SmeehanBlogApi.Quotes
{
    public class Speaker
    {
        /// <summary>
        /// The individual speaking.
        /// </summary>
        public string Person { get; set; }

        /// <summary>
        /// The individual's words.
        /// </summary>
        public string Words { get; set; }
        
        /// <summary>
        /// The order in the quote.
        /// </summary>
        public int Order { get; set; }
    }
}
