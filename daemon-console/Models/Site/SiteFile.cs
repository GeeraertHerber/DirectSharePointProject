using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models
{
    public class SiteFile
    {
        [JsonProperty("mimeType")]
        public string MimeType { get; set; }
        [JsonProperty("hashes")]
        public Hashes Hashes { get; set; }
    }
}
