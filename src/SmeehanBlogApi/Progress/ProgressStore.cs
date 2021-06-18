using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    ///<inheritdoc/>
    public class ProgressStore :IProgressStore
    {
        /// <summary>
        /// Creates and initializes a Project Store. 
        /// </summary>
        /// <param name="dynamodbContext">The context to work with DynamoDb <see cref="IDynamoDBContext"/>.</param>
        /// <param name="client">The client rquired to work with DynamoDb <see cref="IAmazonDynamoDB"/>.</param>
        /// <param name="options">The Options used to work with the Store <see cref="IOptions<QuoteOptions>"/>.</param>
        public ProgressStore(
            IDynamoDBContext dynamodbContext,
            IAmazonDynamoDB client,
            IOptions<ProgressOptions> options)
        {
            _dbContext = dynamodbContext ?? throw new ArgumentNullException(nameof(dynamodbContext), "The IDynamoDBContext was null");
            _client = client ?? throw new ArgumentNullException(nameof(client), "The AmazonDynamoDBClient was null");
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options must be configured.");
        }

        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _client;
        private readonly ProgressOptions _options;

        ///<inheritdoc/>
        public async Task AddProjectAsync(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project), "A Project must be provided");
            }

            await _dbContext.SaveAsync(project).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task BatchStoreAsync(IEnumerable<Project> projects)
        {
            if (projects == null)
            {
                throw new ArgumentNullException(nameof(projects), "quotes cannot be null");
            }

            if (!projects.Any())
            {
                throw new ArgumentException(nameof(projects), "At least one quote must be provided");
            }

            var itemBatch = _dbContext.CreateBatchWrite<Project>();

            foreach (var project in projects)
            {
                itemBatch.AddPutItem(project);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<Project> GetItemAsync(int id)
        {
            var projects = await _dbContext.QueryAsync<Project>(id).GetRemainingAsync();
                        
            if(!projects.Any())
            {
                return null;
            }

            if(projects.Count > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(projects), "Too many projects were returned");
            }

            var activeProject = projects.Where(p => p.Active);
            if (activeProject.Any() & projects.Count() == activeProject.Count())
            {
                return projects.Single(p => p.Active);
            }

            return projects.Single(p => !p.Active);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids)
        {//FUTURE Update this method since it has a sort key
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids), "ids cannot be null");
            }

            if (!ids.Any())
            {
                throw new ArgumentException(nameof(ids), "At least one id must be provided");
            }

            var batchGet = _dbContext.CreateBatchGet<Project>();
            foreach (var id in ids)
            {
                batchGet.AddKey(id);
            }
            await batchGet.ExecuteAsync().ConfigureAwait(false);
            return batchGet.Results;
        }

        ///<inheritdoc/>
        public async Task ModifyProgressAsync(Project project)
        {//FUTURE Update this method since it has a sort key
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.SaveAsync(project);
        }

        ///<inheritdoc/>
        public async Task DeleteProgressAsync(Project project)
        {//FUTURE Update this method since it has a sort key
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.DeleteAsync(project).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.PropertyName))
            {
                throw new ArgumentNullException(nameof(_options.PropertyName), "The property name must be specified");
            }

            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition(_options.PropertyName, ScanOperator.Equal, true));
            var projects = await _dbContext.ScanAsync<Project>(conditions).GetRemainingAsync();
            return projects.OrderBy(x => x.Id);
        }
    }
}