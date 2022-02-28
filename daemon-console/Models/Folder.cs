using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    internal class Folder
    {
        [JsonProperty("childCount")]
        public int ChildCount { get; set; }
    }
}
