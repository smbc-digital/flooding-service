using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Models.ContactDetails;
using StockportGovUK.NetStandard.Models.Mail;
using Xunit;

namespace flooding_service_tests.Helpers
{
    public class MailHelperTests
    {
        private readonly MailHelper _mailHelper;
        private readonly Mock<IMailingServiceGateway> _mockMailingServiceGateway = new Mock<IMailingServiceGateway>();
        private readonly Mock<ILogger<MailHelper>> _mockLogger = new Mock<ILogger<MailHelper>>();

        public MailHelperTests()
        {
            _mailHelper = new MailHelper(_mockMailingServiceGateway.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendEmail_ShouldCallMailingServiceGateway()
        {
            // Arrange
            var floodingRequest = new FloodingRequest
            {
                WhereIsTheFlood = "pavement",
                WhatDoYouWantToReport = "flood",
                Reporter = new ContactDetails
                {
                    EmailAddress = "EmailAddress"
                }
            };

            // Act
            await _mailHelper.SendEmail(floodingRequest, "123456");

            // Assert
            _mockMailingServiceGateway.Verify(_ => _.Send(It.IsAny<Mail>()), Times.Once);
        }

        [Fact]
        public async Task SendEmail_ShouldNotCallMailingServiceGateway_IfTemplateNotFoundForJourneyType()
        {
            // Arrange
            var floodingRequest = new FloodingRequest
            {
                WhereIsTheFlood = "non-existant journey",
                WhatDoYouWantToReport = "flood",
                Reporter = new ContactDetails
                {
                    EmailAddress = "EmailAddress"
                }
            };

            // Act
            await _mailHelper.SendEmail(floodingRequest, "123456");

            // Assert
            _mockMailingServiceGateway.Verify(_ => _.Send(It.IsAny<Mail>()), Times.Never);
        }
    }
}
