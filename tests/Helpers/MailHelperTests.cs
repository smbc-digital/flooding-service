using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Models.ContactDetails;
using StockportGovUK.NetStandard.Models.Enums;
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

        [Theory]
        [InlineData(EMailTemplate.ReportAFloodPublicSpaces, "pavement")]
        [InlineData(EMailTemplate.ReportAFloodPublicSpaces, "road")]
        [InlineData(EMailTemplate.ReportAFloodPublicSpaces, "parkOrFootpath")]
        [InlineData(EMailTemplate.ReportAFloodPrivateSpaces, "privateLand")]
        [InlineData(EMailTemplate.ReportAFloodPrivateSpaces, "business")]
        [InlineData(EMailTemplate.ReportAFloodPrivateSpaces, "home")]
        [InlineData(EMailTemplate.ReportAFloodHighWaterLevels, "highWaterLevels", "highWaterLevels")]
        public void SendEmail_ShouldCallMailingServiceGateway_WithCorrectEmailTemplate_ForJourney(EMailTemplate emailTemplate, string journey, string whatToReport = "flood")
        {
            var callbackValue = new Mail();
            // Arrange
            _mockMailingServiceGateway.Setup(_ => _.Send(It.IsAny<Mail>()))
                .Callback<Mail>(a => callbackValue = a);

            var floodingRequest = new FloodingRequest
            {
                WhereIsTheFlood = journey,
                WhatDoYouWantToReport = whatToReport,
                Reporter = new ContactDetails
                {
                    EmailAddress = "EmailAddress"
                }
            };

            // Act
            _mailHelper.SendEmail(floodingRequest, "123456");

            // Assert
            _mockMailingServiceGateway.Verify(_ => _.Send(It.IsAny<Mail>()), Times.Once);
            Assert.Equal(emailTemplate, callbackValue.Template);
        }

        [Fact]
        public void SendEmail_ShouldNotCallMailingServiceGateway_IfTemplateNotFoundForJourneyType()
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
            _mailHelper.SendEmail(floodingRequest, "123456");

            // Assert
            _mockMailingServiceGateway.Verify(_ => _.Send(It.IsAny<Mail>()), Times.Never);
        }
    }
}
