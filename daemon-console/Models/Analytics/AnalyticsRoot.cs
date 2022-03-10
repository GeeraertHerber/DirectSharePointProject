using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models.Analytics
{
    public class AnalyticsRoot
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("analysisInput")]
        public AnalysisInput AnalysisInput { get; set; }

        [JsonProperty("tasks")]
        public Tasks Tasks { get; set; }
    }
}
