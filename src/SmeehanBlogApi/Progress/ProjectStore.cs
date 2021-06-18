using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    public class ProjectStore :IProjectStore
    {
        public ProjectStore()
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
        ///  AddProject will accept a Project object and create an Item on Amazon DynamoDB
        /// </summary>
        /// <param name="project"></param>
        public async Task AddProjectAsync(Project project)
        {
            await _dbContext.SaveAsync(project).ConfigureAwait(false);
        }

        /// <summary>
        /// The BatchStore Method allows you to store a list of Projects to dynamoDb
        /// </summary>
        /// <param name="projects"></param>
        public async Task BatchStoreAsync<Project>(IEnumerable<Project> projects)
        {
            var itemBatch = _dbContext.CreateBatchWrite<Project>();

            foreach (var project in projects)
            {
                itemBatch.AddPutItem(project);
            }

            await itemBatch.ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a Progress based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Project> GetItem<Project>(int id)
        {
            return await _dbContext.LoadAsync<Project>(id).ConfigureAwait(false);
        }

        /// <summary>
        /// The BatchGet Method allows you to retrieve a list of Projects out of dynamoDb
        /// </summary>
        /// <param name="Progresss"></param>
        public async Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids)
        {
            var batchGet = _dbContext.CreateBatchGet<Project>();
            foreach (var id in ids)
            {
                batchGet.AddKey(id);
            }
            await batchGet.ExecuteAsync().ConfigureAwait(false);
            return batchGet.Results;
        }

        /// <summary>
        /// ModifyProgress  tries to load an existing Projects, modifies and saves it back. If the Item doesn’t exist, it raises an exception
        /// </summary>
        /// <param name="project"></param>
        public async Task ModifyProgressAsync(Project project)
        {
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.SaveAsync(project);
        }

        /// <summary>
        /// Delete Project will remove an item from DynamoDb
        /// </summary>
        /// <param name="project"></param>
        public async Task DeleteProgressAsync(Project project)
        {
            var savedItem = await _dbContext.LoadAsync(project).ConfigureAwait(false);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _dbContext.DeleteAsync(project).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Active Projects will get n number of Progress from DynamoDb
        /// </summary>
        /// <param name="numberToGet"></param>
        /// <param name="beginingId"></param>
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            List<ScanCondition> conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("Active", ScanOperator.Equal, true));
            var projects = await _dbContext.ScanAsync<Project>(conditions).GetRemainingAsync();
            return projects.OrderBy(x => x.Id);
        }
    }
}