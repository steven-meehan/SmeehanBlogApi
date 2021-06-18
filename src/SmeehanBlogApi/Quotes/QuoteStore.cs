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
    public class QuoteStore : IQuoteStore
    {
        public QuoteStore()
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

        /// <summary>
        ///  AddQuote will accept a Quote object and creates an Item on Amazon DynamoDB
        /// </summary>
        /// <param name="quote"></param>
        public async Task AddQuoteAsync(Quote quote)
        {
            await _dbContext.SaveAsync(quote).ConfigureAwait(false);
        }

        /// <summary>
        /// The BatchStore Method allows you to store a list of items of type T to dynamoDb
        /// </summary>
        /// <param name="quotes"></param>
        public async Task BatchStoreAsync(IEnumerable<Quote> quotes)
        {
            var itemBatch = _dbContext.CreateBatchWrite<Quote>();

            foreach (var item in quotes)
            {
                itemBatch.AddPutItem(item);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a Quote based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Quote> GetItem(int id)
        {
            return await _dbContext.LoadAsync<Quote>(id).ConfigureAwait(false);
        }

        /// <summary>
        /// The BatchGet Method allows you to retrieve a list of quotes out of dynamoDb
        /// </summary>
        /// <param name="quotes"></param>
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

        /// <summary>
        /// ModifyQuote  tries to load an existing Quote, modifies and saves it back. If the Item doesn’t exist, it raises an exception
        /// </summary>
        /// <param name="quote"></param>
        public async Task ModifyQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.SaveAsync(quote);
        }

        /// <summary>
        /// Delete Quote will remove an item from DynamoDb
        /// </summary>
        /// <param name="quote"></param>
        public async Task DeleteQuoteAsync(Quote quote)
        {
            var savedItem = await _dbContext.LoadAsync(quote).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.DeleteAsync(quote).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Random Quotes will get n number of quotes from DynamoDb
        /// </summary>
        /// <param name="numberToGet"></param>
        /// <param name="beginingId"></param>
        public async Task<IEnumerable<Quote>> GetRandomQuotesAsync(int numberToGet, int beginingId = 1001)
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



        //Hhelper Methods//
        /// <summary>
        /// This will get the description of the given table, the default value of tableName is Quote
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private async Task<DescribeTableResponse> GetTableDescription(string tableName = "Quote")
        {
            var request = new DescribeTableRequest();
            request.TableName = tableName;
            return await _client.DescribeTableAsync(request).ConfigureAwait(false);
        }
    }
}
