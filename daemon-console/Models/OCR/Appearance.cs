using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json; 


namespace daemon_console.Models.OCR
{
    public class Appearance
    {
        [JsonPropertyName("style")]
        public Style Style { get; set; }
    }
}
