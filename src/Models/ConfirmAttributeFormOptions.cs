using System.Collections.Generic;

namespace flooding_service.Models
{
    public class ConfirmAttributeFormOptions
    {
        public const string ConfigValue = "ConfirmAttributeFormOptions";
        public List<Config> AttributeOptions { get; set; }
    }

    public class Config
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
