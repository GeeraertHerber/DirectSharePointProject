using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    public class WordObject
    {
        [JsonProperty("words")]
        string[] Words { get; set; }
    }
}
