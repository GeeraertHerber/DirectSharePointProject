using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class FileSP
    {
        [JsonProperty("application")]
        public Application Application { get; set; }
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("eTag")]
        public string ETag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("lastModifiedDateTime")]
        public DateTime LastModifiedDateTime { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }
        [JsonProperty("cTag")]
        public string CTag { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }
        [JsonProperty("lastModifiedBy")]
        public LastModifiedBy LastModifiedBy { get; set; }
        [JsonProperty("parentReference")]
        public ParentReference ParentReference { get; set; }
        [JsonProperty("fileSystemInfo")]
        public FileSystemInfo FileSystemInfo { get; set; }
        [JsonProperty("folder")]
        public Folder Folder { get; set; }
    }
}
