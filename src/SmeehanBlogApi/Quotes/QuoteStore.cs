using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
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
            IOptions<QuoteOptions> options,
            ILogger<QuoteStore> logger)
        {
            _dbContext = dynamodbContext ?? throw new ArgumentNullException(nameof(dynamodbContext), "The IDynamoDBContext was null");
            _client = client ?? throw new ArgumentNullException(nameof(client), "The AmazonDynamoDBClient was null");
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options must be configured.");
            _logger = logger ?? throw new ArgumentNullException(nameof(dynamodbContext), "The provided ILogger was null");
        }

        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _client;
        private readonly QuoteOptions _options;
        private readonly ILogger<QuoteStore> _logger;

        ///<inheritdoc/>
        public async Task AddQuoteAsync(Quote quote)
        {
            if (quote == null)
            {
                _logger.LogWarning("The provided quote was null");
                throw new ArgumentNullException(nameof(quote), "A Quote must be provided");
            }

            _logger.LogDebug("Saving the quote to the Database");
            await _dbContext.SaveAsync(quote).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task BatchStoreAsync(IEnumerable<Quote> quotes)
        {
            if (quotes == null)
            {
                _logger.LogWarning($"The provided IEnumerable was null");
                throw new ArgumentNullException(nameof(Quote), "quotes cannot be null");
            }

            if (!quotes.Any())
            {
                _logger.LogWarning($"The provided IEnumerable was empty");
                throw new ArgumentException(nameof(Quote), "At least one quote must be provided");
            }

            var itemBatch = _dbContext.CreateBatchWrite<Quote>();

            foreach (var item in quotes)
            {
                _logger.LogDebug("Quote was added to the Batch Job");
                itemBatch.AddPutItem(item);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
            _logger.LogDebug("Quotes were added to the data store");
        }

        ///<inheritdoc/>
        public async Task<Quote> GetItem(int id)
        {
            var quote = await _dbContext.LoadAsync<Quote>(id).ConfigureAwait(false);

            if (quote == null)
            {
                _logger.LogWarning($"There was no quote for identifier: {id}");
                return null;
            }

            _logger.LogDebug($"Returning quote with the identifier: {id}");
            return quote;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Quote>> BatchGetAsync(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                _logger.LogWarning("The provided ids were null");
                throw new ArgumentNullException(nameof(ids), "ids cannot be null");
            }

            if (!ids.Any())
            {
                _logger.LogWarning("The provided ids is empty");
                throw new ArgumentException(nameof(ids), "At least one id must be provided");
            }

            var batchGet = _dbContext.CreateBatchGet<Quote>();
            foreach (var id in ids)
            {
                _logger.LogDebug("Quote Id was added to the Batch Job");
                batchGet.AddKey(id);
            }
            await batchGet.ExecuteAsync().ConfigureAwait(false);
            _logger.LogDebug("Retrieved the quotes");

            return batchGet.Results;
        }

        ///<inheritdoc/>
        public async Task ModifyQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                _logger.LogWarning($"There was no quote for identifier {quote.Id}");
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _logger.LogDebug($"Updating the quote for identifier {quote.Id}");
            await _dbContext.SaveAsync(quote);
        }

        ///<inheritdoc/>
        public async Task DeleteQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                _logger.LogWarning($"There was no quote for identifier {quote.Id}");
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _logger.LogDebug($"Deleting the quote for identifier {quote.Id}");
            await _dbContext.DeleteAsync(quote).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int numberofQuotesAvailable)
        {
            if(numberofQuotesAvailable < _options.BeginingId)
            {
                _logger.LogWarning("There was an issue getting the total number of Quotes from the DynamoDB Table");
                throw new ArgumentOutOfRangeException(
                    nameof(numberofQuotesAvailable), 
                    "There was an issue getting the total number of Quotes from the DynamoDB Table");
            }

            var ids = new List<int>();

            while (numberToGet > 0)
            {
                var number = new Random().Next(_options.BeginingId, numberofQuotesAvailable);
                _logger.LogInformation($"Quote {numberToGet} will use {number}");

                if (!ids.Contains(number))
                {
                    _logger.LogInformation("The identifier is unique to the list");

                    ids.Add((int)number);
                    numberToGet--;
                }
            }

            _logger.LogInformation("Retrieving the random quotes");
            return await BatchGetAsync(ids).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<DescribeTableResponse> GetTableDescription()
        {
            if (string.IsNullOrWhiteSpace(_options.TableName))
            {
                _logger.LogWarning("The Tabel Name option property was not set");
                throw new ArgumentNullException(nameof(_options.TableName), "The Table Name must be specified");
            }

            var request = new DescribeTableRequest()
            {
                TableName = _options.TableName,
            };

            _logger.LogDebug("Retrieving the Table Description");
            return await _client.DescribeTableAsync(request).ConfigureAwait(false);
        }
    }
}
