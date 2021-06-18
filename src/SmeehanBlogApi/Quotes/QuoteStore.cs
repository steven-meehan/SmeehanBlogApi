using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Quotes
{
    ///<inheritdoc/>
    public class QuoteStore : IQuoteStore
    {
        /// <summary>
        /// Creates and initializes a Quote Store. 
        /// </summary>
        /// <param name="dynamodbContext">The context to work with DynamoDb <see cref="IDynamoDBContext"/>.</param>
        /// <param name="client">The client rquired to work with DynamoDb <see cref="IAmazonDynamoDB"/>.</param>
        /// <param name="options">The Options used to work with the Store <see cref="IOptions<QuoteOptions>"/>.</param>
        public QuoteStore(
            IDynamoDBContext dynamodbContext, 
            IAmazonDynamoDB client,
            IOptions<QuoteOptions> options)
        {
            _dbContext = dynamodbContext ?? throw new ArgumentNullException(nameof(dynamodbContext), "The IDynamoDBContext was null");
            _client = client ?? throw new ArgumentNullException(nameof(client), "The AmazonDynamoDBClient was null");
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options must be configured.");
        }

        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _client;
        private readonly QuoteOptions _options;

        ///<inheritdoc/>
        public async Task AddQuoteAsync(Quote quote)
        {
            if (quote == null)
            {
                throw new ArgumentNullException(nameof(quote), "A Quote must be provided");
            }

            await _dbContext.SaveAsync(quote).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task BatchStoreAsync(IEnumerable<Quote> quotes)
        {
            if (quotes == null)
            {
                throw new ArgumentNullException(nameof(Quote), "quotes cannot be null");
            }

            if (!quotes.Any())
            {
                throw new ArgumentException(nameof(Quote), "At least one quote must be provided");
            }

            var itemBatch = _dbContext.CreateBatchWrite<Quote>();

            foreach (var item in quotes)
            {
                itemBatch.AddPutItem(item);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<Quote> GetItem(int id)
        {
            return await _dbContext.LoadAsync<Quote>(id).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Quote>> BatchGetAsync(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids), "ids cannot be null");
            }

            if (!ids.Any())
            {
                throw new ArgumentException(nameof(ids), "At least one id must be provided");
            }

            var batchGet = _dbContext.CreateBatchGet<Quote>();
            foreach (var id in ids)
            {
                batchGet.AddKey(id);
            }
            await batchGet.ExecuteAsync().ConfigureAwait(false);
            return batchGet.Results;
        }

        ///<inheritdoc/>
        public async Task ModifyQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.SaveAsync(quote);
        }

        ///<inheritdoc/>
        public async Task DeleteQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.DeleteAsync(quote).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int numberofQuotesAvailable)
        {         
            var ids = new List<int>();

            while (numberToGet > 0)
            {
                var number = new Random().Next(_options.BeginingId, numberofQuotesAvailable);
                if (!ids.Contains(number))
                {
                    ids.Add((int)number);
                    numberToGet--;
                }
            }

            return await BatchGetAsync(ids).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<DescribeTableResponse> GetTableDescription()
        {
            var request = new DescribeTableRequest()
            {
                TableName = _options.TableName,
            };

            return await _client.DescribeTableAsync(request).ConfigureAwait(false);
        }
    }
}
