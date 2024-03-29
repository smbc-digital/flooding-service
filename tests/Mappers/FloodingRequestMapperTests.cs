﻿using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using StockportGovUK.NetStandard.Gateways.Models.ContactDetails;
using Xunit;

namespace flooding_service_tests.Mappers
{
    public class FloodingRequestMapperTests
    {
        private readonly FloodingConfiguration _floodingPavementConfiguration;
        private readonly FloodingConfiguration _floodingHomeConfiguration;
        private readonly FloodingConfiguration _floodingRoadConfiguration;
        private readonly Config _roadFloodingLocationConfiguration;
        private readonly Config _pavementFloodingLocationConfiguration;


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

            _roadFloodingLocationConfiguration = new Config
            {
                Type = "road",
                Value = "Road"
            };

            _pavementFloodingLocationConfiguration = new Config
            {
                Type = "pavement",
                Value = "Pavement"
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

            var streetResult = new FloodingAddress
            {
                UniqueId = "654321",
                USRN = "123456",
                Name = "street, place"
            };

            var addressResult = new StockportGovUK.NetStandard.Gateways.Models.Verint.Address
            {
                USRN = "123456",
                Reference = "reference",
                Description = "description"
            };

            var expectedDescription =
                "Where is the flood coming from: river\r\nWhere is the flood: On a pavement\r\nBlocking the pavement: yes\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingPavementConfiguration, streetResult,_pavementFloodingLocationConfiguration);

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
                    EmailAddress = "EmailAddress",
                    Address = new StockportGovUK.NetStandard.Gateways.Models.Addresses.Address
                    {
                        PlaceRef = "123456",
                        SelectedAddress = "SelectedAddress"
                    }
                },
                HowWouldYouLikeToBeContacted = "phone",
                TellUsABoutTheFlood = "It's a flood"
            };

            var streetResult = new FloodingAddress
            {
                USRN = "123456",
                UniqueId = "reference",
                Name = "description"
            };

            var expectedDescription =
                "Where is the flood coming from: river\r\nWhere is the flood: In a home\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingHomeConfiguration, streetResult, null);

            // Assert
            Assert.Equal(2009484, result.EventCode);
            Assert.Equal(_floodingHomeConfiguration.VerintOption.Classification, result.Classification);
            Assert.Equal(_floodingHomeConfiguration.VerintOption.EventTitle, result.EventTitle);
            Assert.Equal(request.Reporter.Address.SelectedAddress, result.Street.Description);
            Assert.Equal(request.Reporter.Address.PlaceRef, result.Street.USRN);
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

            var streetResult = new FloodingAddress
            {
                UniqueId = "654321",
                USRN = "123456",
                Name = "street, place"
            };

            var expectedDescription =
                "Where is the flood coming from: river\r\nWhere is the flood: On a road\r\nBlocking the road: no\r\nTell us about the flood: It's a flood\r\nHow would you like to be contacted: phone\r\n";

            // Act
            var result = request.ToCase(_floodingRoadConfiguration, streetResult, _roadFloodingLocationConfiguration);

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