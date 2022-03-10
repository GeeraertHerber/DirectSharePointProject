using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
