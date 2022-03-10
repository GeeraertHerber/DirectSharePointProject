using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class Parameters
    {
        [JsonProperty("model-version")]
        public string ModelVersion { get; set; }

        [JsonProperty("project-name")]
        public string ProjectName { get; set; }

        [JsonProperty("deployment-name")]
        public string DeploymentName { get; set; }
    }
}
