using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace daemon_console.Models
{
    public class Site
    {
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public string MicrosoftGraphDownloadUrl { get; set; }
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("eTag")]
        public string ETag { get; set; }
        private string _rootSiteUrl;
        [JsonProperty("rootSiteUrl")]
        public string RootSiteUrl { get => _rootSiteUrl; }
        private string _siteId;
        [JsonProperty("siteId")]
        public string SiteId { get => _siteId; }
        [JsonProperty("id")]
        public string Id
        {
            get => $"Root site: {_rootSiteUrl}, site id: {_siteId}";
            set
            {
                string[] splitedString = value.Split(",");
                _rootSiteUrl = splitedString[0];
                _siteId = splitedString[1];
            }
        }
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
        [JsonProperty("file")]
        public SiteFile File { get; set; }
        [JsonProperty("fileSystemInfo")]
        public FileSystemInfo FileSystemInfo { get; set; }

    }
}
