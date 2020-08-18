using System;
using System.Text;
using flooding_service.Controllers.Models;
using StockportGovUK.NetStandard.Models.Verint;

namespace flooding_service.Mappers
{   
    public static class FloodingRequestMapper
    {
        public static FloodingRequest ToCase(this FloodingRequest floodingRequest)
        {
            var crmCase = new Case
            {
                EventCode = 11,
                Classification = "",
                EventTitle = "",
                Customer = new Customer 
                {
                    Forename = floodingRequest.Reporter.FirstName,
                    Surname = floodingRequest.Reporter.LastName,
                    Email = floodingRequest.Reporter.EmailAddress,
                    Telephone = floodingRequest.Reporter.PhoneNumber
                },
                Description = DescriptionBuilder(floodingRequest)
            };

            return floodingRequest;
        }

        private static string DescriptionBuilder(FloodingRequest floodingRequest) 
        {
            var description = new StringBuilder()
                .Append($"What do you want to report: {floodingRequest.WhatDoYouWantToReport}{Environment.NewLine}")
                .Append($"Where is the flooding coming from: {floodingRequest.WhereIsTheFloodingComingFrom}{Environment.NewLine}")
                .Append($"Where is the flood: {floodingRequest.WhereIsTheFlood}{Environment.NewLine}");

            if(!string.IsNullOrWhiteSpace(floodingRequest.IsTheFloodingBlockingTheWholePavementOrCausing))
                description.Append($"Is the flooding blocking the whole pavement: {floodingRequest.IsTheFloodingBlockingTheWholePavementOrCausing}{Environment.NewLine}");

            description.Append($"Tell us about the flood: {floodingRequest.TellUsABoutTheFlood}{Environment.NewLine}")
                .Append($"How would you like to be contacted: {floodingRequest.HowWouldYouLikeToBeContacted}{Environment.NewLine}");

            return description.ToString();
        }
    }
}