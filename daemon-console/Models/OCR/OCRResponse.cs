using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;


namespace daemon_console.Models.OCR
{
    public class OCRResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }

        [JsonPropertyName("lastUpdatedDateTime")]
        public DateTime LastUpdatedDateTime { get; set; }

        [JsonPropertyName("analyzeResult")]
        public AnalyzeResult AnalyzeResult { get; set; }
    }
}
