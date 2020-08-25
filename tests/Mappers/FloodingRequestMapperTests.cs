using flooding_service.Constants;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using Moq;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.ContactDetails;
using Xunit;

namespace flooding_service_tests.Mappers
{
    public class FloodingRequestMapperTests
    {
        private readonly FloodingConfiguration _floodingPavementConfiguration;
        private readonly FloodingConfiguration _floodingHomeConfiguration;
        private readonly FloodingConfiguration _floodingRoadConfiguration;

        public FloodingRequestMapperTests()
        {
            _floodingPavementConfiguration = new FloodingConfiguration
            {
                VerintOption = new Option
                {
                    Classification = "Public Realm >> Highways >> Flooded Roadway",
                    EventTitle = "Flooded pavement",
                    Type = "pavement",
                    EventCode = 2002592
                }
            };

            _floodingRoadConfiguration = new FloodingConfiguration
            {
                VerintOption = new Option
                {
                    Classification = "Public Realm >> Highways >> Flooded Roadway",
                    EventTitle = "Flooded road",
                    Type = "road",
                    EventCode = 2002695
                }
            };

            _floodingHomeConfiguration = new FloodingConfiguration
            {
                VerintOption = new Option
                {
                    Classification = "Public Realm >> Highways >> Flooded Roadway",
                    EventTitle = "Flooded home",
                    Type = "home",
                    EventCode = 2009484
                }
            };
        }

        [Fact]
        public void ToCase_ShouldMapToCase_WhenMapUsed_PavementJourney()
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

            var streetResult = new AddressSearchResult
            {
                UniqueId = "654321",
                USRN = "123456",
                Name = "street, place"
            };

            var expectedDescription =
                "Where is the flooding coming from: river\r\nWhere is the flood: On a pavement\r\nBlocking the pavement: yes\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingPavementConfiguration, streetResult);

            // Assert
            Assert.Equal(2002592, result.EventCode);
            Assert.Equal(_floodingPavementConfiguration.VerintOption.Classification, result.Classification);
            Assert.Equal(_floodingPavementConfiguration.VerintOption.EventTitle, result.EventTitle);
            Assert.Equal(streetResult.Name, result.Street.Description);
            Assert.Equal(streetResult.USRN, result.Street.USRN);
            Assert.Equal(streetResult.UniqueId, result.Street.Reference);
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
                "Where is the flooding coming from: river\r\nWhere is the flood: home\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingHomeConfiguration, It.IsAny<AddressSearchResult>());

            // Assert
            Assert.Equal(2009484, result.EventCode);
            Assert.Equal(_floodingHomeConfiguration.VerintOption.Classification, result.Classification);
            Assert.Equal(_floodingHomeConfiguration.VerintOption.EventTitle, result.EventTitle);
            Assert.Equal(ConfirmConstants.Description, result.Street.Description);
            Assert.Equal(ConfirmConstants.USRN, result.Street.USRN);
            Assert.Equal(expectedDescription, result.Description);
        }

        [Fact]
        public void ToCase_ShouldMapToCase_WhenMapUsed_RoadJourney()
        {
            // Arrange
            var request = new FloodingRequest
            {
                WhereIsTheFlood = "road",
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
                IsTheFloodingBlockingTheWholeRoadOrCausing = "no",
                TellUsABoutTheFlood = "It's a flood"
            };

            var streetResult = new AddressSearchResult
            {
                UniqueId = "654321",
                USRN = "123456",
                Name = "street, place"
            };

            var expectedDescription =
                "Where is the flooding coming from: river\r\nWhere is the flood: On a road\r\nBlocking the road: no\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingRoadConfiguration, streetResult);

            // Assert
            Assert.Equal(2002695, result.EventCode);
            Assert.Equal(_floodingRoadConfiguration.VerintOption.Classification, result.Classification);
            Assert.Equal(_floodingRoadConfiguration.VerintOption.EventTitle, result.EventTitle);
            Assert.Equal(streetResult.Name, result.Street.Description);
            Assert.Equal(streetResult.USRN, result.Street.USRN);
            Assert.Equal(streetResult.UniqueId, result.Street.Reference);
            Assert.Equal(expectedDescription, result.Description);
        }

    }
}