using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    /// <summary>
    /// Exposes CRUD operations to work with a DynamoDb of <see cref="Project"/>.
    /// </summary>
    public interface IProjectStore
    {
        /// <summary>
        ///  Accepts a <see cref="Project"/> and saves it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="project">The complete <see cref="Project"/> to be inserted into DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task AddProjectAsync(Project project);

        /// <summary>
        ///  Accepts a list of <see cref="IEnumerable<Project>"/> and saves it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="projects">The complete list of <see cref="IEnumerable<Project>"/> to be inserted into DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task BatchStoreAsync(IEnumerable<Project> projects);

        /// <summary>
        /// Retrieves a <see cref="Project"/> from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="id">The identifier, an <see cref="int"/> for the <see cref="Project"/> to be retrieved from the DynamoBD Table.</param>
        /// <returns>The requested <see cref="Project"/>.</returns>
        public Task<Project> GetItem(int id);

        /// <summary>
        /// Retrieves a list of <see cref="IEnumerable<Project>"/> from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="ids">A list of identifiers, an <see cref="IEnumerable<int>"/>, for a list od <see cref="IEnumerable<Project>"/> to be retrieved from the DynamoBD Table.</param>
        /// <returns>The requested list of <see cref="IEnumerable<Project>"/>.</returns>
        public Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids);

        /// <summary>
        ///  Accepts a <see cref="Project"/> and updates it in an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="project">The complete <see cref="Project"/> to be updated in the DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task ModifyProgressAsync(Project project);

        /// <summary>
        ///  Accepts a <see cref="Project"/> and deletes it from an Amazon DynamoDB Table.
        /// </summary>
        /// <param name="project">The complete <see cref="Project"/> to be deleted from the DynamoDB Table.</param>
        /// <returns>Completed Task.</returns>
        public Task DeleteProgressAsync(Project project);

        /// <summary>
        /// Retrieves the collection of active <see cref="Project"/> from an Amazon DynamoDb Table.
        /// </summary>
        /// <returns>The list of <see cref="IEnumerable<Project>"/>.</returns>
        public Task<IEnumerable<Project>> GetActiveProjectsAsync();
    }
}
