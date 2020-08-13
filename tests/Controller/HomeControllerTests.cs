using flooding_service.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace flooding_service_tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly Mock<ILogger<HomeController>> _logger = new Mock<ILogger<HomeController>>();
        
        public HomeControllerTests()
        {
            _homeController = new HomeController(_logger.Object);
        }
        
        [Fact]
        public void Post_ShouldReturnOK()
        {
            // Act
            var response = _homeController.Post();
            var statusResponse = response as OkResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(200, statusResponse.StatusCode);
        }
    }
}
