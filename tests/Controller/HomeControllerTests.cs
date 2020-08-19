using flooding_service.Controllers;
using flooding_service.Controllers.Models;
using flooding_service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace flooding_service_tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly Mock<IFloodingService> _mockFloodingService = new Mock<IFloodingService>();
        public HomeControllerTests()
        {
            _homeController = new HomeController(_mockFloodingService.Object);
        }
        
        [Fact]
        public void Post_ShouldReturnOK()
        {
            // Act
            var response = _homeController.Post(new FloodingRequest());
            var statusResponse = response as OkResult;
            
            // Assert
            Assert.NotNull(statusResponse);
            Assert.Equal(200, statusResponse.StatusCode);
        }
    }
}
