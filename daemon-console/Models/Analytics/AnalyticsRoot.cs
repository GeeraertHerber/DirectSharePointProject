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
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        [JsonProperty("lastUpdateDateTime")]
        public DateTime LastUpdateDateTime { get; set; }

        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("expirationDateTime")]
        public DateTime ExpirationDateTime { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("errors")]
        public List<object> Errors { get; set; }
    }
}
