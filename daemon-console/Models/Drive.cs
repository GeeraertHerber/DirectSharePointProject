using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class Drive
    {
        [JsonProperty("@odata.context")]
        public string DataContext { get; set; }
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("description")]
        public string Description   { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("lastModifiedDateTime")]
        public DateTime LastModifiedDateTime { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
        [JsonProperty("driveType")]
        public string DriveType { get; set; }
        [JsonProperty("CreatedBy")]
        public CreatedBy CreatedBy { get; set; }
        [JsonProperty("lastModifiedBy")]
        public LastModifiedBy LastModifiedBy { get; set; }
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("quota")]
        public Quota Quota { get; set; }
    }
}
