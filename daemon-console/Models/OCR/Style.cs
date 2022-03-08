using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace daemon_console.Models.OCR
{
    public class Style
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
