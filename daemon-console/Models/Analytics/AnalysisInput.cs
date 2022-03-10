using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class AnalysisInput
    {
        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }
    }
}
