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
        public DateTime createdDateTime { get; set; }
        public string eTag { get; set; }
        private string _rootSiteUrl;
        public string rootSiteUrl { get => _rootSiteUrl; }
        private string _siteId;
        public string siteId { get => _siteId; }
        public string id
        {
            get => $"Root site: {_rootSiteUrl}, site id: {_siteId}";
            set
            {
                string[] splitedString = value.Split(",");
                _rootSiteUrl = splitedString[0];
                _siteId = splitedString[1];
            }
        }

        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string cTag { get; set; }
        public int size { get; set; }
        public CreatedBy createdBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public ParentReference parentReference { get; set; }
        public File file { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }

    }
}
