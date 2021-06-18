namespace SmeehanBlogApi.Quotes
{
    /// <summary>
    /// This configures the code to work with the Quote Stack.
    /// </summary>
    public class QuoteOptions
    {
        /// <summary>
        /// This flag enables switching between the <see cref="QuoteStore"/> and the <see cref="MockQuoteStore"/>
        /// during local development.
        /// </summary>
        public bool MockStore { get; set; } = false;

        /// <summary>
        /// Soecifies the lowest Identifier in the Quote Table.
        /// </summary>
        public int BeginingId { get; } = 1001;

        /// <summary>
        /// Specifies the Quote Table name.
        /// </summary>
        public string TableName { get; set; } = "Quote";
    }
}
