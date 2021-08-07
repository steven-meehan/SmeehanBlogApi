using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmeehanBlogApi.Quotes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Tests
{
    [TestClass]
    public class QuoteStoreTests
    {
        private IDynamoDBContext _dynamodbContext;
        private IAmazonDynamoDB _client;
        private IQuoteStore _quoteStore;
        private IOptions<QuoteOptions> _quoteOptions;
        private Quote _quote = new Quote();
        private List<Quote> _randomQuotes = null;
        private ILogger<QuoteStore> _logger;

        [TestInitialize]
        public void Setup()
        {
            var credentials = new BasicAWSCredentials("", "");
            var config = new AmazonDynamoDBConfig()
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };
            _client = new AmazonDynamoDBClient(credentials, config);
            _dynamodbContext = Mock.Of<IDynamoDBContext>();

            _quoteOptions = Options.Create(new QuoteOptions());
            _logger = Mock.Of<ILogger<QuoteStore>>();
            _quoteStore = new QuoteStore(_dynamodbContext, _client, _quoteOptions, _logger);

            var listOfQuotes = File.ReadAllText("../../../DataSets/ListOfQuotes.json");
            _randomQuotes = JsonConvert.DeserializeObject<IEnumerable<Quote>>(listOfQuotes).ToList();
        }

        [TestMethod]
        public void Constructor_NullIDynamoDBContext_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new QuoteStore(null, _client, _quoteOptions, _logger));

        [TestMethod]
        public void Constructor_NullIAmazonDynamoDB_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new QuoteStore(_dynamodbContext, null, _quoteOptions, _logger));

        [TestMethod]
        public void Constructor_NullIOptions_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new QuoteStore(_dynamodbContext, _client, null, _logger));

        [TestMethod]
        public void Constructor_NullLogger_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new QuoteStore(_dynamodbContext, _client, _quoteOptions, null));

        [TestMethod]
        public void DeleteQuoteAsync_QuoteNotInTable_ThrowsAmazonDynamoDBException()
        {
            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(It.IsAny<Quote>(), new CancellationToken()))
                .Returns(Task.FromResult((Quote)null));
            Assert.ThrowsExceptionAsync<AmazonDynamoDBException>(() => _quoteStore.DeleteQuoteAsync(_quote));
        }

        [TestMethod]
        public void DeleteQuoteAsync_QuoteInTable_ReturnsDeleteItemResponse()
        {
            _quote = _randomQuotes.First();

            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(_quote, new CancellationToken()))
                .Returns(Task.FromResult(_quote));
            Mock.Get(_dynamodbContext)
                .Setup(m => m.DeleteAsync(It.IsAny<Quote>(), new CancellationToken()))
                .Returns(Task.CompletedTask);

            var response = _quoteStore.DeleteQuoteAsync(_quote);

            Assert.AreEqual(response.IsCompletedSuccessfully, true);
        }

        [TestMethod]
        public void ModifyQuoteAsync_QuoteNotInTable_ThrowsAmazonDynamoDBException()
        {
            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(It.IsAny<Quote>(), new CancellationToken()))
                .Returns(Task.FromResult((Quote)null));
            Assert.ThrowsExceptionAsync<AmazonDynamoDBException>(() => _quoteStore.ModifyQuoteAsync(_quote));
        }

        [TestMethod]
        public void ModifyQuoteAsync_QuoteInTable_ReturnsDeleteItemResponse()
        {
            _quote = _randomQuotes.First();

            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(_quote, new CancellationToken()))
                .Returns(Task.FromResult(_quote));
            Mock.Get(_dynamodbContext)
                .Setup(m => m.SaveAsync(It.IsAny<Quote>(), new CancellationToken()))
                .Returns(Task.CompletedTask);

            var response = _quoteStore.ModifyQuoteAsync(_quote);

            Assert.AreEqual(response.IsCompletedSuccessfully, true);
        }

        [TestMethod]
        public void AddQuoteAsync_NullQuote_ThrowArgumentNullException() => 
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _quoteStore.AddQuoteAsync(null));

        [TestMethod]
        public void BatchStoreAsync_NullQuotes_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _quoteStore.BatchStoreAsync(null));

        [TestMethod]
        public void BatchStoreAsync_EmptyQuotes_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _quoteStore.BatchStoreAsync(new List<Quote>()));

        [TestMethod]
        public void BatchGetAsync_NullIds_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _quoteStore.BatchGetAsync(null));

        [TestMethod]
        public void BatchGetAsync_EmptyIds_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _quoteStore.BatchGetAsync(new List<int>()));

        [TestMethod]
        public void GetRandomQuotesAsync_NumberofQuotesAvailableSetTooLow_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _quoteStore.GetRandomQuotesAsync(1001, 0));
    }
}
