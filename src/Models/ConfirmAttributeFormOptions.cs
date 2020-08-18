using System.Collections.Generic;
using StockportGovUK.NetStandard.Extensions.VerintExtensions.VerintOnlineFormsExtensions.ConfirmIntegrationFromExtensions;

namespace flooding_service.Models
{
    public class ConfirmAttributeFormOptions : ConfirmIntegrationFormOptions
    {
        public const string ConfigValue = "ConfirmAttributeFormOptions";

        public List<Config> RiverOrCulvertedWaterConfig { get; set; }

        public List<Config> EventId { get; set; }
    }

    public class Config
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }
}
