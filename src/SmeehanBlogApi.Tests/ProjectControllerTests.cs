using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SmeehanBlogApi.Controllers;
using SmeehanBlogApi.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Tests
{
    [TestClass]
    public class ProjectControllerTests
    {
        private IProgressStore _progressStore;
        private List<Project> _allProjects = null;
        private ProgressController _quotesController;

        [TestInitialize]
        public void Setup()
        {
            var listOfProjects = File.ReadAllText("../../../DataSets/ListOfProjects.json");
            _allProjects = JsonConvert.DeserializeObject<IEnumerable<Project>>(listOfProjects).ToList();

            _progressStore = Mock.Of<IProgressStore>();
            _quotesController = new ProgressController(_progressStore);
        }

        [TestMethod]
        public void Constructor_NullQuoteStore_ThrowArgumentNullException() =>
            Assert.ThrowsException<ArgumentNullException>(() => new ProgressController(null));

        [TestMethod]
        public void GetProjectAsync_NotFoundId_ReturnsNull()
        {
            var project = _allProjects.Where(q => q.Id == 1).SingleOrDefault();
            Mock.Get(_progressStore).Setup(m => m.GetItemAsync(1)).Returns(Task.FromResult(project));
            var actionResult = _quotesController.GetProjectAsync(1).Result;

            var notFoundResult = actionResult as NotFoundObjectResult;

            Assert.IsNull(notFoundResult);
        }

        [TestMethod]
        [DataRow(1001)]
        [DataRow(1003)]
        [DataRow(1006)]
        [DataRow(1007)]
        public void GetProjectAsync_Id_ReturnsProject(int id)
        {
            var project = _allProjects.Where(q => q.Id == id).Single();
            Mock.Get(_progressStore).Setup(m => m.GetItemAsync(id)).Returns(Task.FromResult(project));
            var actionResult = _quotesController.GetProjectAsync(id).Result;

            var okResult = actionResult as OkObjectResult;
            var response = okResult.Value as Project;

            Assert.IsNotNull(response);

            Assert.AreEqual(response.Id, project.Id);
            Assert.AreEqual(response.Series, project.Series);
            Assert.AreEqual(response.Status, project.Status);
            Assert.AreEqual(response.Title, project.Title);
            Assert.AreEqual(response.Type, project.Type);
            Assert.AreEqual(response.Active, project.Active);
        }

        [TestMethod]
        public void GetActiveProjectsAsync_AsIs_AllActiveProject()
        {
            var projects = _allProjects.Where(q => q.Active == true);
            Mock.Get(_progressStore).Setup(m => m.GetActiveProjectsAsync()).Returns(Task.FromResult(projects));

            var response = _progressStore.GetActiveProjectsAsync().Result;

            Assert.AreEqual(response.Count(), projects.Count());
        }
    }
}
