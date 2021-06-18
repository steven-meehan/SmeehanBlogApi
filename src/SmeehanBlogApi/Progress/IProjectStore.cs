using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    public interface IProjectStore
    {
        public Task AddProjectAsync(Project project);
        public Task BatchStoreAsync<Project>(IEnumerable<Project> projects);
        public Task<Project> GetItem<Project>(int id);
        public Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids);
        public Task ModifyProgressAsync(Project project);
        public Task DeleteProgressAsync(Project project);
        public Task<IEnumerable<Project>> GetActiveProjectsAsync();

    }
}
