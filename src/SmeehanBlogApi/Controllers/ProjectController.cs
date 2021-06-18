using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmeehanBlogApi.Progress;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/progress")]
    [EnableCors("MyPolicy")]
    public class ProjectController : Controller
    {
        public ProjectController(IProgressStore projectStore)
        {
            _projectStore = projectStore;
        }
        private IProgressStore _projectStore;

        [HttpGet]
        [Route("active")]
        public async Task<IEnumerable<Project>> GetActiveProjects()
        {
            return await _projectStore.GetActiveProjectsAsync();
        }
    }
}