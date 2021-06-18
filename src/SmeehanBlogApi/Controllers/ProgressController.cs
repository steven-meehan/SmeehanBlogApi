using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmeehanBlogApi.Progress;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/progress")]
    [EnableCors("MyPolicy")]
    public class ProgressController : Controller
    {
        public ProgressController(IProgressStore projectStore)
        {
            _projectStore = projectStore ?? throw new ArgumentNullException(nameof(projectStore), "The provided IQuoteStore was null");
        }

        private IProgressStore _projectStore;

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectAsync(int id)
        {
            Project project = null;

            try
            {
                project = await _projectStore.GetItemAsync(id);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                return NotFound("Could not locate project");
            }

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _projectStore.GetActiveProjectsAsync();
        }
    }
}