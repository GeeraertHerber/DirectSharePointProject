using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class Quota
    {
        [JsonProperty("deleted")]
        public int Deleted { get; set; }
        [JsonProperty("remaining")]
        public long Remaining { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("total")]
        public long Total { get; set; }
        [JsonProperty("used")]
        public int Used { get; set; }
    }
}
