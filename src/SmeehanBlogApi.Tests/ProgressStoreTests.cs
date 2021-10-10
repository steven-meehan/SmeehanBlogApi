using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmeehanBlogApi.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Tests
{
    [TestClass]
    public class ProgressStoreTests
    {
        private IDynamoDBContext _dynamodbContext;
        private IAmazonDynamoDB _client;
        private IProgressStore _progressStore;
        private IOptions<ProgressOptions> _progressOptions;
        private Project _project = new Project();
        private List<Project> _allProjects = null;
        private ILogger<ProgressStore> _logger;

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

            _progressOptions = Options.Create(new ProgressOptions());
            _logger = Mock.Of<ILogger<ProgressStore>>();
            _progressStore = new ProgressStore(_dynamodbContext, _client, _progressOptions, _logger);

            var listOfProjects= File.ReadAllText("../../../DataSets/ListOfProjects.json");
            _allProjects = JsonConvert.DeserializeObject<IEnumerable<Project>>(listOfProjects).ToList();
        }

        [TestMethod]
        public void Constructor_NullIDynamoDBContext_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new ProgressStore(null, _client, _progressOptions, _logger));

        [TestMethod]
        public void Constructor_NullIAmazonDynamoDB_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new ProgressStore(_dynamodbContext, null, _progressOptions, _logger));

        [TestMethod]
        public void Constructor_NullIOptions_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new ProgressStore(_dynamodbContext, _client, null, _logger));

        [TestMethod]
        public void Constructor_NullLogger_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new ProgressStore(_dynamodbContext, _client, _progressOptions, null));

        [TestMethod]
        public void AddProjectAsync_NullProject_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.AddProjectAsync(null));

        [TestMethod]
        public void BatchStoreAsync_NullProjects_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.BatchStoreAsync(null));

        [TestMethod]
        public void BatchStoreAsync_EmptyProjects_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.BatchStoreAsync(new List<Project>()));

        //FUTURE
        //Currently QueryAsync is not mockable, and while there are work arounds, I would prefer to wait until
        //the library has been fixed.
        //Implement below methods
        //[TestMethod]
        //public void GetItemAsync_ProjectNotInTable_Null()
        //[TestMethod]
        //public void GetItemAsync_MoreThanOneProjectInTable_ThrowArgumentOutOfRangeException()
        //[TestMethod]
        //public void GetItemAsync_ProjectActive_Null()
        //[TestMethod]
        //public void GetItemAsync_ProjectInactive_Null()

        [TestMethod]
        public void BatchGetItemAsync_NullIds_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.BatchGetAsync(null));

        [TestMethod]
        public void BatchGetAsync_EmptyIds_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.BatchGetAsync(new List<int>()));

        [TestMethod]
        public void ModifyProjectAsync_ProjectNotInTable_ThrowsAmazonDynamoDBException()
        {
            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(It.IsAny<Project>(), new CancellationToken()))
                .Returns(Task.FromResult((Project)null));
            Assert.ThrowsExceptionAsync<AmazonDynamoDBException>(() => _progressStore.ModifyProgressAsync(_project));
        }

        [TestMethod]
        public void ModifyProjectAsync_ProjectInTable_ReturnsDeleteItemResponse()
        {
            _project = _allProjects.First();

            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(_project, new CancellationToken()))
                .Returns(Task.FromResult(_project));
            Mock.Get(_dynamodbContext)
                .Setup(m => m.SaveAsync(It.IsAny<Project>(), new CancellationToken()))
                .Returns(Task.CompletedTask);

            var response = _progressStore.ModifyProgressAsync(_project);

            Assert.AreEqual(response.IsCompletedSuccessfully, true);
        }

        [TestMethod]
        public void DeleteProjectAsync_ProjectNotInTable_ThrowsAmazonDynamoDBException()
        {
            Mock.Get(_dynamodbContext)
                .Setup(m => m.LoadAsync(It.IsAny<Project>(), new CancellationToken()))
                .Returns(Task.FromResult((Project)null));
            Assert.ThrowsExceptionAsync<AmazonDynamoDBException>(() => _progressStore.DeleteProgressAsync(_project));
        }

        [TestMethod]
        public void GetActiveProjectsAsync_PropertyNameNotSet_ThrowArgumentNullException() =>
            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _progressStore.GetActiveProjectsAsync());

        //FUTURE
        //Currently AsyncSearch is not mockable, and while there are work arounds, I would prefer to wait until
        //the library has been fixed.
        //Implement below method
        //[TestMethod]
        //public void GetActiveProjectsAsync_PropertyNameSet_ListOfActiveProjects()
    }
}
