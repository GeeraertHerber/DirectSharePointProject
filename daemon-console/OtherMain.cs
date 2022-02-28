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
                        if (!site.webUrl.Contains("/personal"))
                        {
                            string siteUrl = ApiCaller.GetDriveBySite(site.siteId);
                            JObject driveResult = ApiManager.RunAsync(siteUrl).GetAwaiter().GetResult();
                            //Console.WriteLine(site.id);
                            if (driveResult != null)
                            {
                                Drive driveObject = (Drive)ApiManager.ConvertToDriveObject(driveResult);

                                string fileUrl = ApiCaller.GetFilesByDrive(driveObject.webUrl);
                                JObject files = ApiManager.RunAsync(fileUrl).GetAwaiter().GetResult();
                                if (files != null)
                                {
                                    Console.WriteLine(files.ToString());
                                }
                            }
                        }
                        
                        //Console.WriteLine($"Site name: {site.name} \t\t Site ID: {site.siteId}");
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
    }
}
