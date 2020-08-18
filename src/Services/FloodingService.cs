using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Mail;

namespace flooding_service.Services
{
    public interface IFloodingService
    {
        Task<string> CreateCase(FloodingRequest request);
    }

    public class FloodingService : IFloodingService
    {
        private readonly IVerintServiceGateway _verintServiceGateway;
        private readonly IMailingServiceGateway _mailingServiceGateway;
        private readonly PavementVerintOptions _pavementVerintOptions;
        private readonly ConfirmAttributeFormOptions _confirmAttributeFormOptions;

        public FloodingService(IVerintServiceGateway verintServiceGateway, IMailingServiceGateway mailingServiceGateway, PavementVerintOptions pavementVerintOptions, ConfirmAttributeFormOptions confirmAttributeFormOptions)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailingServiceGateway = mailingServiceGateway;
            _pavementVerintOptions = pavementVerintOptions;
            _confirmAttributeFormOptions = confirmAttributeFormOptions;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            var crmCase = request.ToCase(_pavementVerintOptions, _confirmAttributeFormOptions);
            var confirmIntegrationFormOptions = request.ToConfirmFormOptions(_confirmAttributeFormOptions);

            var verintRequest = crmCase.ToConfirmIntegrationFormCase(confirmIntegrationFormOptions);
            var caseResult = await _verintServiceGateway.CreateVerintOnlineFormCase(verintRequest);

            await _mailingServiceGateway.Send(new Mail
            {
                Payload = JsonConvert.SerializeObject(new
                {
                    Subject = "Report a flood - submission",
                    Reference = caseResult.ResponseContent.VerintCaseReference,
                    RecipientAddress = request.Reporter.EmailAddress
                }),
                Template = EMailTemplate.ReportAFloodPublicSpaces
            });

            return caseResult.ResponseContent.VerintCaseReference;
        }
    }
}