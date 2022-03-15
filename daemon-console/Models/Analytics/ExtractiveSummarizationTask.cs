using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class ExtractiveSummarizationTask
    {
        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }
        [JsonProperty("lastUpdateDateTime")]
        public DateTime LastUpdateDateTime { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("results")]
        public Results Results { get; set; }
    }
}
