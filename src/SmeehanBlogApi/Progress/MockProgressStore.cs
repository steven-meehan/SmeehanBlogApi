using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Progress
{
    ///<inheritdoc/>
    public class MockProgressStore : IProgressStore
    {
        public MockProgressStore(IOptions<ProgressOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options must be configured.");
        }

        private ProgressOptions _options;

        ///<inheritdoc/>
        public Task AddProjectAsync(Project project)
        {
            _data.Add(project);
            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public Task BatchStoreAsync(IEnumerable<Project> projects)
        {
            _data.AddRange(projects);
            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public Task<Project> GetItemAsync(int id)
        {
            return Task.FromResult(_data.Where<Project>(p => p.Id == id).SingleOrDefault());
        }

        ///<inheritdoc/>
        public Task<IEnumerable<Project>> BatchGetAsync(IEnumerable<int> ids)
        {
            var projects = new List<Project>();

            foreach (var id in ids)
            {
                var project = _data.Where(p => p.Id == id).SingleOrDefault();
                if (project != null)
                {
                    projects.Add(project);
                }
            }

            return Task.FromResult<IEnumerable<Project>>(projects);
        }

        ///<inheritdoc/>
        public Task ModifyProgressAsync(Project project)
        {
            var existingProject = _data.Where(p => p.Id == project.Id).SingleOrDefault();
            if (existingProject == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _data.Remove(existingProject);
            _data.Add(project);

            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public Task DeleteProgressAsync(Project project)
        {
            var existingProject = _data.Where(p => p.Id == project.Id).SingleOrDefault();
            if (existingProject == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            _data.Remove(existingProject);

            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.PropertyName))
            {
                throw new ArgumentNullException(nameof(_options.PropertyName), "The property name must be specified");
            }

            var activeProjects = _data.Where(p => p.Active == true).OrderBy(p => p.Id);

            return Task.FromResult<IEnumerable<Project>>(activeProjects);
        }

        private List<Project> _data = new List<Project>()
        {
            new Project()
            {
                Id = 1000,
                Active = false,
                Title = "Survival",
                Type = 2,
                Series = null,
                Status = 1
            },
            new Project()
            {
                Id = 1001,
                Active = true,
                Title = "Aftermath",
                Type = 1,
                Series = null,
                Status = 2
            },
            new Project()
            {
                Id = 1002,
                Active = true,
                Title = "Discovery",
                Type = 4,
                Series = "",
                Status = 3
            },
            new Project()
            {
                Id = 1003,
                Active = false,
                Title = "Crossroads",
                Type = 4,
                Series = "Harrison & Sylvia",
                Status = 4
            },
            new Project()
            {
                Id = 1004,
                Active = true,
                Title = "Conspiracies",
                Type = 4,
                Series = "Harrison & Sylvia",
                Status = 5
            },
            new Project()
            {
                Id = 1005,
                Active = true,
                Title = "Arrival",
                Type = 3,
                Series = null,
                Status = 6
            },
            new Project()
            {
                Id = 1006,
                Active = false,
                Title = "Hero Call",
                Type = 2,
                Series = null,
                Status = 7
            },
            new Project()
            {
                Id = 1007,
                Active = false,
                Title = "Mounting Tensions",
                Type = 4,
                Series = "Tergaria",
                Status = 8
            },
            new Project()
            {
                Id = 1008,
                Active = true,
                Title = "Fortunes",
                Type = 4,
                Series = "Tergaria",
                Status = 7
            },
            new Project()
            {
                Id = 1009,
                Active = false,
                Title = "Transformation",
                Type = 2,
                Series = null,
                Status = 6
            }
        };

    }
}
