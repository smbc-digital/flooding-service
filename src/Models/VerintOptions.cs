using System.Collections.Generic;

namespace flooding_service.Models
{
    public class VerintOptions
    {
        public const string ConfigValue = "VerintOptions";
       
        public List<Option> Options { get; set; }

        public List<Config> FloodingLocations { get; set; }
    }

    public class Option
    {
        public int EventCode { get; set; }
        public string Classification { get; set; }
        public string EventTitle { get; set; }
        public string Type { get; set; }
        public string ServiceCode { get; set; }
        public string SubjectCode { get; set; }
        public string ClassCode { get; set; }
    }
}