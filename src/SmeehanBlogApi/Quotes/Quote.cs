using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace SmeehanBlogApi.Quotes
{
    [DynamoDBTable("Quote")]
    public class Quote
    {
        /// <summary>
        /// The identifier for the quote
        /// </summary>
        [DynamoDBHashKey]
        public int Id { get; set; }

        /// <summary>
        /// The collection of speakers.
        /// </summary>
        [DynamoDBProperty]
        public List<Speaker> Speakers { get; set; }
        
        /// <summary>
        /// The source of the quote.
        /// </summary>
        [DynamoDBProperty]
        public Source Source { get; set; }
    }
}
