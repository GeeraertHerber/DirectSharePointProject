using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace daemon_console.Models.Analytics
{
    public class Results
    {
        [JsonPropertyName("documents")]
        public List<Document> Documents { get; set; }

        [JsonPropertyName("errors")]
        public List<object> Errors { get; set; }

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; }
    }
}
