using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmeehanBlogApi.Controllers;
using SmeehanBlogApi.Quotes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Tests
{
    [TestClass]
    public class QuotesControllerTests
    {
        private IQuoteStore _quoteStore;
        private List<Quote> _randomQuotes = null;
        private DescribeTableResponse _describeTableResponse;
        private QuotesController _quotesController;

        [TestInitialize]
        public void Setup()
        {
            var listOfQuotes = File.ReadAllText("../../../DataSets/ListOfQuotes.json");
            _randomQuotes = JsonConvert.DeserializeObject<IEnumerable<Quote>>(listOfQuotes).ToList();

            _quoteStore = Mock.Of<IQuoteStore>();
            _quotesController = new QuotesController(_quoteStore);

            _describeTableResponse = new DescribeTableResponse()
            {
                Table = new TableDescription()
                {
                    ItemCount = _randomQuotes.Count()
                }
            };
        }

        [TestMethod]
        public void Constructor_NullQuoteStore_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new QuotesController(null));

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void GetRandomQuoteAsync_X_ReturnsListOfQuotes(int x)
        {
            Mock.Get(_quoteStore).Setup(m => m.GetTableDescription()).Returns(Task.FromResult(_describeTableResponse));
            Mock.Get(_quoteStore).Setup(m => m.GetRandomQuotesAsync(x)).Returns(Task.FromResult(_randomQuotes.Take(x)));
            var okResult = _quotesController.GetRandomQuoteAsync(x).Result as OkObjectResult;
            var response = okResult.Value as IEnumerable<Quote>;

            var responseArray = response.ToArray();
            var randomQuotesArray = _randomQuotes.ToArray();

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Count(), x);

            for (int i = 0; i < x; i++)
            {
                Assert.AreEqual(responseArray[i].Id, randomQuotesArray[i].Id);
                Assert.AreEqual(responseArray[i].Source.Series, randomQuotesArray[i].Source.Series);
                Assert.AreEqual(responseArray[i].Source.Story, randomQuotesArray[i].Source.Story);
                Assert.AreEqual(responseArray[i].Speakers.Count, randomQuotesArray[i].Speakers.Count);
                Assert.AreEqual(responseArray[i].Speakers.Single().Order, randomQuotesArray[i].Speakers.Single().Order);
                Assert.AreEqual(responseArray[i].Speakers.Single().Person, randomQuotesArray[i].Speakers.Single().Person);
                Assert.AreEqual(responseArray[i].Speakers.Single().Words, randomQuotesArray[i].Speakers.Single().Words);
            }
        }

        [TestMethod]
        public void GetRandomQuoteAsync_TooManyQuoteInTable_ThrowArgumentOutOfRangeException()
        {
            _describeTableResponse = new DescribeTableResponse()
            {
                Table = new TableDescription()
                {
                    ItemCount = long.MaxValue
                }
            };

            Mock.Get(_quoteStore).Setup(m => m.GetTableDescription()).Returns(Task.FromResult(_describeTableResponse));

            Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _quotesController.GetRandomQuoteAsync(2, 1001));
        }

        [TestMethod]
        [DataRow(1000)]
        [DataRow(1001)]
        [DataRow(1002)]
        [DataRow(1003)]
        public void GetQuoteAsync_Id_ReturnsQuote(int id)
        {
            var quote = _randomQuotes.Where(q => q.Id == id).Single();
            Mock.Get(_quoteStore).Setup(m => m.GetItem(id)).Returns(Task.FromResult(quote));
            var actionResult = _quotesController.GetQuoteAsync(id).Result;

            var okResult = actionResult as OkObjectResult;
            var response = okResult.Value as Quote;

            Assert.IsNotNull(response);
            
            Assert.AreEqual(response.Id, quote.Id);
            Assert.AreEqual(response.Source.Series, quote.Source.Series);
            Assert.AreEqual(response.Source.Story, quote.Source.Story);
            Assert.AreEqual(response.Speakers.Count, quote.Speakers.Count);
            Assert.AreEqual(response.Speakers.Single().Order, quote.Speakers.Single().Order);
            Assert.AreEqual(response.Speakers.Single().Person, quote.Speakers.Single().Person);
            Assert.AreEqual(response.Speakers.Single().Words, quote.Speakers.Single().Words);
        }

        [TestMethod]
        public void GetQuoteAsync_NotFoundId_ReturnsNull()
        {
            var quote = _randomQuotes.Where(q => q.Id == 1).SingleOrDefault();
            Mock.Get(_quoteStore).Setup(m => m.GetItem(1)).Returns(Task.FromResult(quote));
            var actionResult = _quotesController.GetQuoteAsync(1).Result;

            var notFoundResult = actionResult as NotFoundObjectResult;

            Assert.IsNull(notFoundResult);
        }
    }
}
