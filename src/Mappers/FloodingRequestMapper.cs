using System;
using System.Text;
using flooding_service.Constants;
using flooding_service.Controllers.Models;
using flooding_service.Extensions;
using flooding_service.Models;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint;
using Street = StockportGovUK.NetStandard.Models.Verint.Street;

namespace flooding_service.Mappers
{
    public static class FloodingRequestMapper
    {
        public static Case ToCase(
            this FloodingRequest floodingRequest, 
           FloodingConfiguration floodingConfiguration, 
            AddressSearchResult streetResult)
        {
            var crmCase = new Case
            {
                EventCode = floodingConfiguration.VerintOption.EventCode,
                Classification = floodingConfiguration.VerintOption.Classification,
                EventTitle = floodingConfiguration.VerintOption.EventTitle,
                Customer = new Customer 
                {
                    Forename = floodingRequest.Reporter.FirstName,
                    Surname = floodingRequest.Reporter.LastName,
                    Email = floodingRequest.Reporter.EmailAddress,
                    Telephone = floodingRequest.Reporter.PhoneNumber
                },
                Description = DescriptionBuilder(floodingRequest),
                RaisedByBehaviour = RaisedByBehaviourEnum.Individual,
            };

            if (floodingRequest.DidNotUseMap)
            {
                crmCase.Street = new Street
                {
                    USRN = ConfirmConstants.USRN,
                    Description = ConfirmConstants.Description,
                    Reference = ConfirmConstants.USRN
                };
            }
            else
            {
                crmCase.AssociatedWithBehaviour = AssociatedWithBehaviourEnum.Street;
                crmCase.Street = new Street
                {
                    USRN = streetResult.USRN,
                    Description = streetResult.Name,
                    Reference = string.IsNullOrEmpty(streetResult.UniqueId) ? null : streetResult.UniqueId
                };
            }

            return crmCase;
        }

        private static string DescriptionBuilder(FloodingRequest floodingRequest) 
        {
            var description = new StringBuilder();

            if (floodingRequest.WhatDoYouWantToReport.Equals("flood"))
            {
                description.Append($"Where is the flooding coming from: {floodingRequest.WhereIsTheFloodingComingFrom.WhereIsTheFloodingComingFromToReadableText()}{Environment.NewLine}")
                    .Append($"Where is the flood: {floodingRequest.WhereIsTheFlood.WhereIsTheFloodToReadableText()}{Environment.NewLine}");
            }

            if(!string.IsNullOrWhiteSpace(floodingRequest.IsTheFloodingBlockingTheWholePavementOrCausing))
                description.Append($"Blocking the pavement: {floodingRequest.IsTheFloodingBlockingTheWholePavementOrCausing}{Environment.NewLine}");

            if(!string.IsNullOrWhiteSpace(floodingRequest.IsTheFloodingBlockingTheWholeRoadOrCausing))
                description.Append($"Blocking the road: {floodingRequest.IsTheFloodingBlockingTheWholeRoadOrCausing}{Environment.NewLine}");

            description.Append($"Tell us about the flood: {floodingRequest.TellUsABoutTheFlood}{Environment.NewLine}")
                .Append($"How would you like to be contacted: {floodingRequest.HowWouldYouLikeToBeContacted}{Environment.NewLine}");

            return description.ToString();
        }
    }
}