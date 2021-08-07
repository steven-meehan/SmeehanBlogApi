using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
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
            IOptions<ProgressOptions> options,
            ILogger<ProgressStore> logger)
        {
            _dbContext = dynamodbContext ?? throw new ArgumentNullException(nameof(dynamodbContext), "The IDynamoDBContext was null");
            _client = client ?? throw new ArgumentNullException(nameof(client), "The AmazonDynamoDBClient was null");
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options must be configured.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "The provided ILogger was null");
        }

        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _client;
        private readonly ProgressOptions _options;
        private readonly ILogger<ProgressStore> _logger;

        ///<inheritdoc/>
        public async Task AddProjectAsync(Project project)
        {
            if (project == null)
            {
                _logger.LogWarning("The provided project was null");
                throw new ArgumentNullException(nameof(project), "A Project must be provided");
            }

            _logger.LogDebug("Saving the project to the Database");
            await _dbContext.SaveAsync(project).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task BatchStoreAsync(IEnumerable<Project> projects)
        {
            if (projects == null)
            {
                _logger.LogWarning($"The provided IEnumerable was null");
                throw new ArgumentNullException(nameof(projects), "quotes cannot be null");
            }

            if (!projects.Any())
            {
                _logger.LogWarning($"The provided IEnumerable was empty");
                throw new ArgumentException(nameof(projects), "At least one quote must be provided");
            }

            var itemBatch = _dbContext.CreateBatchWrite<Project>();

            foreach (var project in projects)
            {
                _logger.LogDebug("Project was added to the Batch Job");
                itemBatch.AddPutItem(project);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
            _logger.LogDebug("Projects were added to the data store");
        }

        ///<inheritdoc/>
        public async Task<Project> GetItemAsync(int id)
        {
            var projects = await _dbContext.QueryAsync<Project>(id).GetRemainingAsync();
                        
            if(!projects.Any())
            {
                _logger.LogWarning($"There was no project for identifier: {id}");
                return null;
            }

            if(projects.Count > 1)
            {
                _logger.LogWarning($"There were multiple projects returned for identifier: {id}");
                throw new ArgumentOutOfRangeException(nameof(projects), "Too many projects were returned");
            }

            var activeProject = projects.Where(p => p.Active);
            if (activeProject.Any() & projects.Count() == activeProject.Count())
            {
                _logger.LogDebug("Returning active project");
                return projects.Single(p => p.Active);
            }

            _logger.LogDebug("Returning active project");
            return projects.Single(p => !p.Active);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids)
        {//FUTURE Update this method since it has a sort key
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

            var batchGet = _dbContext.CreateBatchGet<Project>();
            foreach (var id in ids)
            {
                _logger.LogDebug("Project Id was added to the Batch Job");
                batchGet.AddKey(id);
            }

            await batchGet.ExecuteAsync().ConfigureAwait(false);
            _logger.LogDebug("Retrieved the projects");

            return batchGet.Results;
        }

        ///<inheritdoc/>
        public async Task ModifyProgressAsync(Project project)
        {//FUTURE Update this method since it has a sort key
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                _logger.LogWarning($"There was no project for identifier {project.Id}");
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _logger.LogDebug($"Updating the project for identifier {project.Id}");
            await _dbContext.SaveAsync(project);
        }

        ///<inheritdoc/>
        public async Task DeleteProgressAsync(Project project)
        {//FUTURE Update this method since it has a sort key
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                _logger.LogWarning($"There was no project for identifier {project.Id}");
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _logger.LogDebug($"Deleting the project for identifier {project.Id}");
            await _dbContext.DeleteAsync(project).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.PropertyName))
            {
                _logger.LogWarning("The Property Name option property was not set");
                throw new ArgumentNullException(nameof(_options.PropertyName), "The property name must be specified");
            }


            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition(_options.PropertyName, ScanOperator.Equal, true));
            var projects = await _dbContext.ScanAsync<Project>(conditions).GetRemainingAsync();

            if(projects == null || !projects.Any())
            {
                _logger.LogWarning("There were no active projects returned");
                return null;
            }

            _logger.LogInformation("Returning active projects");
            return projects.OrderBy(x => x.Id);
        }
    }
}