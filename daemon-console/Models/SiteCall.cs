using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    public class SiteCall
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<Site> value { get; set; }
    }
}
