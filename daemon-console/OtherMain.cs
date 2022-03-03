using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates; //Only import this if you are using certificate
using System.Threading.Tasks;
using System.IO;
using daemon_console.Models;

namespace daemon_console
{
    class OtherMain
    {
        public static void MainTester()
        {
            
            try
            {
                //string url = ApiCaller.GetSite();
                string url = ApiCaller.GetSite();
                JObject apiResult = ApiManager.RunAsync(url).GetAwaiter().GetResult();
                SiteCall siteObject = (SiteCall)ApiManager.ConvertToSiteObjects(apiResult);
                //Console.WriteLine(siteObject.value.First().siteId);
                if (siteObject != null)
                {
                    foreach (var site in siteObject.value)
                    {
                        if (!site.webUrl.Contains("/personal") && site.name != null && site.webUrl.Contains("ProjectFalcon-UXtest"))
                        {
                            url = $"sites/{site.siteId}/lists?expand=fields";
                            JObject result = ApiManager.RunAsync(url).GetAwaiter().GetResult();
                            Console.WriteLine(result.ToString());
                            /*string siteUrl = ApiCaller.GetDriveBySite(site.siteId);
                            JObject driveResult = ApiManager.RunAsync(siteUrl).GetAwaiter().GetResult();
                            //Console.WriteLine(site.id);
                            if (driveResult != null )
                            {
                                Drive driveObject = (Drive)ApiManager.ConvertToDriveObject(driveResult);

                                string dirUrl = ApiCaller.GetFilesByDrive(driveObject.Id);
                                if (dirUrl != null)
                                {
                                    GetInsidesDir(dirUrl, driveObject);
                                }
                                
                            }*/
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }   
  
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        private static void GetInsidesDir(string url, Drive driveObject = null)
        {
            JObject apiResult = ApiManager.RunAsync(url).GetAwaiter().GetResult();
            DirRoot dirObject = (DirRoot)ApiManager.ConvertToFile(apiResult);
            foreach(var dirContent in dirObject.Files)
            {
                if (dirContent.size > 0)
                {
                   /* Console.WriteLine(dirContent.name);
                    Console.WriteLine(dirContent.eTag);*/
                    if (dirContent.name.Contains("Hello_world"))
                    {
                        string fileUrl = $"/drives/{driveObject.Id}/items/{dirContent.id}";
                        GetFile(fileUrl);
                    }
                }
                else if(dirContent.folder.ChildCount > 0)
                {
                    Console.WriteLine(dirContent.folder.ChildCount);
                    GetInsidesDir(dirContent.webUrl);
                }
            }
        }
        private static void GetFile(string url)
        {
            JObject apiResult = ApiManager.RunAsync(url).GetAwaiter().GetResult();
            Console.WriteLine(apiResult.ToString());
        }
    }
}
