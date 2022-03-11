using System;
using System.Collections.Generic;
using System.Text;
using daemon_console.Models;
namespace daemon_console.Models
{
    internal class ApiCaller
    {
       
        public static string UrlCreator(string url, bool beta = false)
        {
            string standardUrl = "https://graph.microsoft.com/";
            if (beta)
            {
                standardUrl += "beta/";
            }
            else
            {
                standardUrl += "v1.0/";
            }
            standardUrl += url;
            return standardUrl;
        }

        public static string GetSite(string siteId = "root")
        {
            string url;
            url = UrlCreator($"sites/{siteId}");
            return url;
        }
        
        public static string GetDriveBySite(string siteId, bool standard = true)
        {
            string url; 
            if (standard)
            {
                //Gets the standard drive for this site
                url = UrlCreator($"sites/{siteId}/drive");
            }
            else
            {
                //Gets all the drives connected to this site
                url = UrlCreator($"sites/{siteId}/drives");
            }
            return url;
        }

        public static string GetFilesByDrive(string driveId, string pathRelative = "")
        {
            string url;
            url = UrlCreator($"/drives/{driveId}/root:/{pathRelative}:/children");
            return url;
        }
    }
}
