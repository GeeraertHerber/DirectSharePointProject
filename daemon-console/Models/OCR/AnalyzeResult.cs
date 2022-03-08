using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace daemon_console.Models.OCR
{
    public class AnalyzeResult
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; }

        [JsonPropertyName("readResults")]
        public List<ReadResult> ReadResults { get; set; }
    }
}
