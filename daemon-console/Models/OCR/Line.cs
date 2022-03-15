using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace daemon_console.Models.OCR
{
    public class Line
    {
        [JsonPropertyName("boundingBox")]
        public List<float> BoundingBox { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("appearance")]
        public Appearance Appearance { get; set; }

        [JsonPropertyName("words")]
        public List<Word> Words { get; set; }
    }
}
