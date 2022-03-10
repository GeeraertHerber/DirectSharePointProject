using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace daemon_console.Models.OCR
{
    public class ReadResult
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("angle")]
        public double Angle { get; set; }

        [JsonPropertyName("width")]
        public float Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("lines")]
        public List<Line> Lines { get; set; }
    }
}
