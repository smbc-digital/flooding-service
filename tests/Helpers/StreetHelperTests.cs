using System.Collections.Generic;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using Moq;
using StockportGovUK.NetStandard.Gateways.Response;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Addresses;
using Xunit;

namespace flooding_service_tests.Helpers
{
    public class StreetHelperTests
    {
        private readonly IStreetHelper _streetHelper;
        private readonly Mock<IVerintServiceGateway> _mockVerintGateway = new Mock<IVerintServiceGateway>();

        private readonly string _street = "Street";
        private readonly string _usrn = "123456";

        public StreetHelperTests()
        {
            _streetHelper = new StreetHelper(_mockVerintGateway.Object);
        }

        [Fact]
        public async Task GetStreetUniqueId_ShouldCallVerintServiceGateway()
        {
            // Arrange
            var map = new Map
            {
                Street = $"{_street}, {_usrn}"
            };

            _mockVerintGateway
                .Setup(_ => _.GetStreetByUsrn(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<List<AddressSearchResult>>());

            // Act
            await _streetHelper.GetStreetUniqueId(map);

            // Assert
            _mockVerintGateway.Verify(_ => _.GetStreetByUsrn(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetStreetUniqueId_ShouldReturnFirstResult_IfResponseNotNull()
        {
            // Arrange
            var map = new Map
            {
                Street = $"{_street}, {_usrn}"
            };

            var firstResult = new AddressSearchResult
            {
                UniqueId = "654321",
                USRN = _usrn,
                Name = "Street, Town"
            };

            var secondResult = new AddressSearchResult
            {
                UniqueId = "777777",
                USRN = "555555",
                Name = "Different Street"
            };

            _mockVerintGateway
                .Setup(_ => _.GetStreetByUsrn(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<List<AddressSearchResult>>
                {
                    ResponseContent = new List<AddressSearchResult>
                    {
                       firstResult,
                       secondResult
                    }
                });

            // Act
            var result = await _streetHelper.GetStreetUniqueId(map);

            // Assert
            Assert.Equal(firstResult.UniqueId, result.UniqueId);
            Assert.Equal(firstResult.USRN, result.USRN);
            Assert.Equal(firstResult.Name, result.Name);
        }

        [Fact]
        public async Task GetStreetUniqueId_ShouldReturnRequestAsAddressSearchResult_AndUniqueIdAsNull_IfNoResults()
        {
            // Arrange
            var map = new Map
            {
                Street = $"{_street}, {_usrn}"
            };

            _mockVerintGateway
                .Setup(_ => _.GetStreetByUsrn(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponse<List<AddressSearchResult>>
                {
                    ResponseContent = null
                });

            // Act
            var result = await _streetHelper.GetStreetUniqueId(map);

            // Assert
            Assert.Null(result.UniqueId);
            Assert.Equal(_usrn, result.USRN);
            Assert.Equal(_street, result.Name);
        }
    }
}
