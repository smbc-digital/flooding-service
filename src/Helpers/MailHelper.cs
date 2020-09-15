using System;
using flooding_service.Controllers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Mail;

namespace flooding_service.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(FloodingRequest floodingRequest, string caseReference);
    }

    public class MailHelper : IMailHelper
    {
        private readonly IMailingServiceGateway _mailingServiceGateway;
        private readonly ILogger<MailHelper> _logger;

        public MailHelper(IMailingServiceGateway mailingServiceGateway, ILogger<MailHelper> logger)
        {
            _mailingServiceGateway = mailingServiceGateway;
            _logger = logger;
        }

        public void SendEmail(FloodingRequest floodingRequest, string caseReference)
        {
            if (string.IsNullOrEmpty(floodingRequest.Reporter.EmailAddress))
                return;

            EMailTemplate template;
            var templateNotFound = false;

            var value = floodingRequest.WhatDoYouWantToReport.Equals("highWaterLevels") 
                ? floodingRequest.WhatDoYouWantToReport 
                : floodingRequest.WhereIsTheFlood;

            switch (value)
            {
                case "pavement":
                case "road":
                case "parkOrFootpath":
                    template = EMailTemplate.ReportAFloodPublicSpaces;
                    break;
                case "privateLand":
                case "home":
                case "business":
                    template = EMailTemplate.ReportAFloodPrivateSpaces;
                    break;
                case "highWaterLevels":
                    template = EMailTemplate.ReportAFloodHighWaterLevels;
                    break;
                default:
                    templateNotFound = true;
                    template = EMailTemplate.BaseTemplate;
                    _logger.LogWarning($"MailHelper:: SendEmail:: Email not sent, no email template found for {floodingRequest.WhereIsTheFlood} journey");
                    break;
            }

            if (!templateNotFound)
            {
                try
                {
                    _mailingServiceGateway.Send(new Mail
                    {
                        Payload = JsonConvert.SerializeObject(new
                        {
                            Subject = "Report a flood - submission",
                            Reference = caseReference,
                            RecipientAddress = floodingRequest.Reporter.EmailAddress
                        }),
                        Template = template
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}