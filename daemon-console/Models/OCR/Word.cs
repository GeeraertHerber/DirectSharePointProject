using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace daemon_console.Models.OCR
{
    public  class Word
    {
        [JsonPropertyName("boundingBox")]
        public List<float> BoundingBox { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
