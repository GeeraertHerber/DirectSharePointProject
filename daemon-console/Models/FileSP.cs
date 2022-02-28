using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class FileSP
    {
        [JsonProperty("application")]
        public Application application { get; set; }
        public DateTime createdDateTime { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string cTag { get; set; }
        public int size { get; set; }
        public CreatedBy createdBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public ParentReference parentReference { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
        public Folder folder { get; set; }
    }
}
