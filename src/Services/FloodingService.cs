using System;
using System.Linq;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Mappers;
using flooding_service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<PavementVerintOptions> _pavementVerintOptions;
        private readonly IOptions<ConfirmAttributeFormOptions> _confirmAttributeFormOptions;
        private readonly ILogger<FloodingService> _logger;

        public FloodingService(IVerintServiceGateway verintServiceGateway,
                                IMailingServiceGateway mailingServiceGateway,
                                IOptions<PavementVerintOptions> pavementVerintOptions,
                                IOptions<ConfirmAttributeFormOptions> confirmAttributeFormOptions, ILogger<FloodingService> logger)
        {
            _verintServiceGateway = verintServiceGateway;
            _mailingServiceGateway = mailingServiceGateway;
            _pavementVerintOptions = pavementVerintOptions;
            _confirmAttributeFormOptions = confirmAttributeFormOptions;
            _logger = logger;
        }

        public async Task<string> CreateCase(FloodingRequest request)
        {
            try
            {
                var street = request.Map?.Street.Split(',').ToList();
                var streetSearchResults = await _verintServiceGateway.GetStreetByReference(street.SkipLast(1)
                    .Aggregate("", (x, y) => x + y + ',').Trim(','));
                try
                {
                    var usrn = await _verintServiceGateway.GetStreet(street.Last().Trim());
                    if (usrn.ResponseContent != null)
                    {
                        _logger.LogWarning($"FloodingService:: CreateCase:: Street found = {JsonConvert.SerializeObject(usrn)}");
                    }
                    else
                    {
                        _logger.LogWarning("FloodingService:: CreateCase:: No street found");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"FloodingService:: CreateCase:: Failed on call to Verint GetStreet, error: {ex.Message}");
                }

                var streetResult = streetSearchResults.ResponseContent == null || streetSearchResults.ResponseContent.Count > 1
                    ? null
                    : streetSearchResults.ResponseContent.FirstOrDefault();

                var crmCase = request.ToCase(_pavementVerintOptions.Value, _confirmAttributeFormOptions.Value, streetResult);
                var confirmIntegrationFormOptions = request.ToConfirmFormOptions(_confirmAttributeFormOptions.Value);
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
            catch (Exception ex)
            {
                throw new Exception($"FloodingService:: CreateCase, Failed to create case, exception: {ex.Message}");
            }
        }
    }
}