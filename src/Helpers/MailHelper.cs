using System;
using System.Threading.Tasks;
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
        Task SendEmail(FloodingRequest floodingRequest, string caseReference);
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

        public async Task SendEmail(FloodingRequest floodingRequest, string caseReference)
        {
            EMailTemplate template;
            var templateNotFound = false;

            switch (floodingRequest.WhereIsTheFlood)
            {
                case "pavement": 
                    template = EMailTemplate.ReportAFloodPublicSpaces;
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
                    await _mailingServiceGateway.Send(new Mail
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
                    _logger.LogError($"MailHelper:: SendMail:: Email failed to send with exception: {ex.Message}");
                }
            }
            
        }
    }
}
