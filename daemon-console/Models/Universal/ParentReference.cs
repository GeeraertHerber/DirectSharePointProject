using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; 

namespace daemon_console.Models
{
    public class ParentReference
    {
        [JsonProperty("driveId")]
        public string DriveId { get; set; }
        [JsonProperty("driveType")]
        public string DriveType { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
