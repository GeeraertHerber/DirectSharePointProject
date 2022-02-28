using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; 

namespace daemon_console.Models
{
    internal class FileRoot
    {
        [JsonProperty("@odata.context")]
        public string DataContext { get; set; }
        [JsonProperty("value")]
        public List<FileSP> Files { get; set; }
    }
}
