using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Quotes
{
    /// <summary>
    /// Exposes CRUD operations to work with a DynamoDb of <see cref="Quote"/>.
    /// </summary>
    public interface IQuoteStore
    {
        /// <summary>
        ///  Accepts a <see cref="Quote"/> and saves it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="quote">The complete <see cref="Quote"/> to be inserted into DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task AddQuoteAsync(Quote quote);

        /// <summary>
        ///  Accepts a list of <see cref="IEnumerable<Quote>"/> and saves it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="quote">The complete list of <see cref="IEnumerable<Quote>"/> to be inserted into DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task BatchStoreAsync(IEnumerable<Quote> quotes);

        /// <summary>
        /// Retrieves a <see cref="Quote"/> from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="id">The identifier, an <see cref="int"/> for the <see cref="Quote"/> to be retrieved from the DynamoBD Table.</param>
        /// <returns>The requested <see cref="Quote"/>.</returns>
        public Task<Quote> GetItem(int id);

        /// <summary>
        /// Retrieves a list of <see cref="IEnumerable<Quote>"/> from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="ids">A list of identifiers, an <see cref="IEnumerable<int>"/>, for a list od <see cref="IEnumerable<Quote>"/> to be retrieved from the DynamoBD Table.</param>
        /// <returns>The requested list of <see cref="IEnumerable<Quote>"/>.</returns>
        public Task<IEnumerable<Quote>> BatchGetAsync(IEnumerable<int> ids);

        /// <summary>
        ///  Accepts a <see cref="Quote"/> and updates it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="quote">The complete <see cref="Quote"/> to be updated in the DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task ModifyQuoteAsync(Quote quote);

        /// <summary>
        ///  Accepts a <see cref="Quote"/> and deletes it from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="quote">The complete <see cref="Quote"/> to be deleted from the DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task DeleteQuoteAsync(Quote quote);

        /// <summary>
        /// Retrieves a random collection of <see cref="Quote"/> from an Amazon DynamoDb Table.
        /// </summary>
        /// <param name="numberToGet">The total number of <see cref="Quote"/> to retrieve.</param>
        /// <returns>The random list of <see cref="IEnumerable<Quote>"/>.</returns>
        public Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet);

        /// <summary>
        /// Retrieves table information from Amazon DynamoDB. 
        /// </summary>
        /// <returns>The table's description <see cref="DescribeTableResponse"/></returns>
        public Task<DescribeTableResponse> GetTableDescription();
    }
}
