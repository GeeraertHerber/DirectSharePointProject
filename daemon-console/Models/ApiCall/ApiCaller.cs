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
            if (pathRelative == "")
            {
                url = UrlCreator($"drives/{driveId}/root/children");
                
            }
            else
            {
                url = UrlCreator($"drives/{driveId}/root:{pathRelative}:/children");
            }
           
            Console.WriteLine(url);
            return url;
        }

        public static string GetConvertFilePDF(string driveId, string fileName, string parentReference = null)
        {
            //https://graph.microsoft.com/beta/drives/b!vFMTUH3YJ0iHv5pUatRpqXdN3S3rz75Kvhyrf0kHHx9SiVwv01P_Solc6sU6SAea/root:/Open.docx:/content?format=pdf
            //Call works only in BETA!!! Very important

            fileName = fileName.Replace(" ", "%20");
            string url; 
            if (parentReference != null)
            {
                 url = $"drives/{driveId}/root:{parentReference}/{fileName}:/content?format=pdf";
            }
            else
            {
                url = $"drives/{driveId}/root:/{fileName}:/content?format=pdf";
            }
           
            url = UrlCreator(url, true);
            return url;
        }
        public static string GetPDF(string driveId, string itemId)
        {
            //fileName = fileName.Replace(" ", "%20");
            string url;
            url = $"drives/{driveId}/items/{itemId}/content";
            url = UrlCreator(url, true);
            return url;
        }
    }
}
