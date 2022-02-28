using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class Drive
    {
        [JsonProperty("@odata.context")]
        public string dataContext { get; set; }
        public DateTime createdDateTime { get; set; }
        public string description   { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string driveType { get; set; }
        public CreatedBy CreatedBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }  
        public Owner owner { get; set; }
        public Quota quota { get; set; }
    }
}
