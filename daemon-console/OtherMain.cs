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
                SiteCall siteObject = JsonConvert.DeserializeObject<SiteCall>(apiResult.ToString());
                //Console.WriteLine(siteObject.value.First().siteId);
                if (siteObject != null)
                {
                    foreach (var site in siteObject.Value)
                    {
                        if (!site.webUrl.Contains("/personal") && site.name != null && site.webUrl.Contains("Retail"))
                        {
                            Console.WriteLine(site.siteId);
                            /*
                            url = $"sites/{site.siteId}/columns";
                            JObject result = ApiManager.RunAsync(url).GetAwaiter().GetResult();
                            Console.WriteLine(result.ToString());*/
                            string siteUrl = ApiCaller.GetDriveBySite(site.siteId);
                            JObject driveResult = ApiManager.RunAsync(siteUrl).GetAwaiter().GetResult();
                            //Console.WriteLine(site.id);
                            if (driveResult != null)
                            {
                                Drive driveObject = JsonConvert.DeserializeObject<Drive>(driveResult.ToString());

                                string dirUrl = ApiCaller.GetFilesByDrive(driveObject.Id);
                                if (dirUrl != null)
                                {
                                    GetInsidesDir(dirUrl, driveObject);
                                }

                            }
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
            DirRoot dirObject = JsonConvert.DeserializeObject<DirRoot>(apiResult.ToString());
            foreach(var dirContent in dirObject.Files)
            {
                if (dirContent.Size > 0 )
                {
                   /* Console.WriteLine(dirContent.name);
                    Console.WriteLine(dirContent.eTag);*/
                    
                    GetFile(driveObject, dirContent);
                                   }    
                else if(dirContent.Folder.ChildCount > 0)
                {
                    Console.WriteLine(dirContent.Folder.ChildCount);
                    GetInsidesDir(dirContent.WebUrl);
                }
            }
        }

        private static void GetFile(Drive driveObject, FileSP fileObject)
        {
            string fileUrl = $"/drives/{driveObject.Id}/items/{fileObject.Id}?expand=fields"; //
            if (fileObject.Name.Contains("Open"))
            {
                GetPDF(driveObject, fileObject);
            }
            
        }
        private static void GetPDF(Drive driveObject, FileSP fileObject)
        {
            string pdfUrl = $"/drives/{driveObject.Id}/root:/{fileObject.Name.Replace(" ", "%")}:/content?format=pdf";
            JObject apiResult = ApiManager.RunAsync(pdfUrl, true, true).GetAwaiter().GetResult();
            
            Console.WriteLine(apiResult.ToString());
        }
    }
}
