using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmeehanBlogApi.Progress;

namespace SmeehanBlogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/progress")]
    [EnableCors("MyPolicy")]
    public class ProjectController : Controller
    {
        public ProjectController(IProjectStore projectStore)
        {
            _projectStore = projectStore;
        }
        private IProjectStore _projectStore;

        [HttpGet]
        [Route("active")]
        public async Task<IEnumerable<Project>> GetActiveProjects()
        {
            return await _projectStore.GetActiveProjectsAsync();
        }
    }
}