using System.Collections.Generic;

namespace flooding_service.Models
{
    public class ConfirmAttributeFormOptions
    {
        public const string ConfigValue = "ConfirmAttributeFormOptions";

        public List<Config> FloodingSourceReported { get; set; }

        public List<Config> CommercialOrDomestic { get; set; }

        public List<Config> FloodLocationInProperty { get; set; }
    }

    public class Config
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}