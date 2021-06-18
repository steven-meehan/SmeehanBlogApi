using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Quotes
{
    public interface IQuoteStore
    {
        public Task AddQuoteAsync(Quote quote);
        public Task BatchStoreAsync(IEnumerable<Quote> quotes);
        public Task<Quote> GetItem(int id);
        public Task<IEnumerable<Quote>> BatchGetAsync(IEnumerable<int> ids);
        public Task ModifyQuoteAsync(Quote quote);
        public Task DeleteQuoteAsync(Quote quote);
        public Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int beginingId = 1001);
    }
}
