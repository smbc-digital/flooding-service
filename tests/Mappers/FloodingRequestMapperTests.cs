using System.Collections.Generic;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using StockportGovUK.NetStandard.Models.ContactDetails;
using Xunit;

namespace flooding_service_tests.Mappers
{
    public class FloodingRequestMapperTests
    {
        private readonly ConfirmAttributeFormOptions _confirmAttributeFormOptions;
        private readonly PavementVerintOptions _pavementVerintOptions;

        public FloodingRequestMapperTests()
        {
            _confirmAttributeFormOptions = new ConfirmAttributeFormOptions
            {
                EventId = new List<Config>
                {
                    new Config
                    {
                        Type = "pavement",
                        Value = "2002592"
                    },new Config
                    {
                        Type = "home",
                        Value = "2009484"
                    }

                }
            };

            _pavementVerintOptions = new PavementVerintOptions
            {
                Classification = "Public Realm >> Highways >> Flooded Roadway",
                EventTitle = "Flooded pavement"
            };
        }

        [Fact]
        public void ToCase_ShouldMapToCase_WhenMapUsed()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "pavement",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                Map = new Map
                {
                    Lat = "lat",
                    Lng = "lng",
                    Street = "street, place, 123456"
                },
                Reporter = new ContactDetails
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    PhoneNumber = "PhoneNumber",
                    EmailAddress = "EmailAddress"
                },
                HowWouldYouLikeToBeContacted = "phone",
                IsTheFloodingBlockingTheWholePavementOrCausing = "yes",
                TellUsABoutTheFlood = "It's a flood"
            };

            var expectedDescription =
                "What do you want to report: flood\r\nWhere is the flooding coming from: river\r\nWhere is the flood: pavement\r\nIs the flooding blocking the whole pavement: yes\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_pavementVerintOptions, _confirmAttributeFormOptions);

            // Assert
            Assert.Equal(2002592, result.EventCode);
            Assert.Equal(_pavementVerintOptions.Classification, result.Classification);
            Assert.Equal(_pavementVerintOptions.EventTitle, result.EventTitle);
            Assert.Equal("street, place", result.Street.Description);
            Assert.Equal("123456", result.Street.USRN);
            Assert.Equal(expectedDescription, result.Description);
        }

        [Fact]
        public void ToCase_ShouldMapToCase_WhenMapNotUsed()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "home",
                WhereIsTheFloodingComingFrom = "river",
                WhatDoYouWantToReport = "flood",
                Reporter = new ContactDetails
                {
                    FirstName = "FirstName",
                    LastName = "LastName",
                    PhoneNumber = "PhoneNumber",
                    EmailAddress = "EmailAddress"
                },
                HowWouldYouLikeToBeContacted = "phone",
                TellUsABoutTheFlood = "It's a flood"
            };

            var expectedDescription =
                "What do you want to report: flood\r\nWhere is the flooding coming from: river\r\nWhere is the flood: home\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_pavementVerintOptions, _confirmAttributeFormOptions);

            // Assert
            Assert.Equal(2009484, result.EventCode);
            Assert.Equal(_pavementVerintOptions.Classification, result.Classification);
            Assert.Equal(_pavementVerintOptions.EventTitle, result.EventTitle);
            Assert.Equal(ConfirmConstants.Description, result.Street.Description);
            Assert.Equal(ConfirmConstants.USRN, result.Street.USRN);
            Assert.Equal(expectedDescription, result.Description);
        }
    }
}
