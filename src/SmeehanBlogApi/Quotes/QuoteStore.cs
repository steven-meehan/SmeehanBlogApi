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
        public QuoteStore()
        /// <summary>
        /// Creates and initializes a Quote Store. 
        /// </summary>
        {
            _client = new AmazonDynamoDBClient();

            _dbContext = new DynamoDBContext(_client, new DynamoDBContextConfig
            {
                //Setting the Consistent property to true ensures that you'll always get the latest 
                ConsistentRead = true,
                SkipVersionCheck = true
            });
        }

        private readonly DynamoDBContext _dbContext;
        private readonly AmazonDynamoDBClient _client;

        ///<inheritdoc/>
        public async Task AddQuoteAsync(Quote quote)
        {
            await _dbContext.SaveAsync(quote).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task BatchStoreAsync(IEnumerable<Quote> quotes)
        {
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

        public async Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int beginingId = 1001)
        ///<inheritdoc/>
        {
            int endingId = 0;
            var response = await GetTableDescription().ConfigureAwait(false);

            if (response.Table.ItemCount < int.MaxValue)
            {
                endingId = Convert.ToInt32(response.Table.ItemCount) + (beginingId - 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException("There are too many quotes in the database");
            }

            var ids = new List<int>();

            while (numberToGet > 0)
            {
                var number = new Random().Next(beginingId, endingId);
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
