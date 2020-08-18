using System;
using System.Linq;
using System.Text;
using flooding_service.Controllers.Models;
using flooding_service.Models;
using StockportGovUK.NetStandard.Models.Verint;

namespace flooding_service.Mappers
{   
    public static class FloodingRequestMapper
    {
        public static Case ToCase(this FloodingRequest floodingRequest, PavementVerintOptions verintOptions, ConfirmAttributeFormOptions confirmAttributeFormOptions)
        {
            var crmCase = new Case
            {
                EventCode = int.Parse(confirmAttributeFormOptions.EventId.FirstOrDefault(_ => _.Type.Equals(floodingRequest.WhereIsTheFlood)).Value),
                Classification = verintOptions.Classification,
                EventTitle = verintOptions.EventTitle,
                Customer = new Customer 
                {
                    Forename = floodingRequest.Reporter.FirstName,
                    Surname = floodingRequest.Reporter.LastName,
                    Email = floodingRequest.Reporter.EmailAddress,
                    Telephone = floodingRequest.Reporter.PhoneNumber
                },
                Description = DescriptionBuilder(floodingRequest),
                RaisedByBehaviour = RaisedByBehaviourEnum.Individual,
                AssociatedWithBehaviour = AssociatedWithBehaviourEnum.Street
            };

            if (floodingRequest.Map != null && string.IsNullOrEmpty(floodingRequest.Map.Street))
            {
                crmCase.Street.USRN = "99999999";
                crmCase.Street.Description = "No street reference found";
            }
            else
            {
                var street = floodingRequest.Map?.Street.Split(',').ToList();
                crmCase.Street.USRN = street.Last();
                crmCase.Street.Description = street.SkipLast(1).ToString();
            }

            return crmCase;
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