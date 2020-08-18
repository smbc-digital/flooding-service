using System.Collections.Generic;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Gateways.VerintService;
using StockportGovUK.NetStandard.Models.Models.Verint.VerintOnlineForm;

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
        private readonly ConfirmIntegrationFormOptions _confirmIntegrationFormOptions;

        public FloodingService(IVerintServiceGateway verintServiceGateway, IMailingServiceGateway mailingServiceGateway, PavementVerintOptions pavementVerintOptions, ConfirmIntegrationFormOptions confirmIntegrationFormOptions)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailingServiceGateway = mailingServiceGateway;
            _pavementVerintOptions = pavementVerintOptions;
            _confirmIntegrationFormOptions = confirmIntegrationFormOptions;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            var crmCase = request.ToCase(_pavementVerintOptions);

            var verintRequest = new VerintOnlineFormRequest
            {
                VerintCase = crmCase,
                FormName = "confirm_integrationform",
                FormData = new Dictionary<string, string>()
            };

            var caseResult = _verintServiceGateway.CreateVerintOnlineFormCase(verintRequest);

            //create case in verint
            //generate email model
            //send email
            //return caseRef
            return "12345";
        }
    }
}