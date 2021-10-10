using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public ProgressController(
            IProgressStore projectStore,
            ILogger<ProgressController> logger)
        {
            _projectStore = projectStore ?? throw new ArgumentNullException(nameof(projectStore), "The provided IQuoteStore was null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "The provided ILogger was null");
        }

        private IProgressStore _projectStore;
        private ILogger<ProgressController> _logger;

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectAsync(int id)
        {
            Project project = null;

            try
            {
                _logger.LogTrace("Retrieving data from database");
                project = await _projectStore.GetItemAsync(id);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError($"There was an error retrieving the project for {id}", ex);
                return NotFound("Could not locate project");
            }

            if (project == null)
            {
                _logger.LogWarning($"The provided identifier: {id}, provided no results");
                return NotFound();
            }

            return Ok(project);
        }

        [HttpGet]
        [Route("active")]
        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            _logger.LogTrace("Retrieving data from database");
            return await _projectStore.GetActiveProjectsAsync();
        }
    }
}