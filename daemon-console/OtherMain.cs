﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using daemon_console.Models;
using daemon_console.Models.OCR;
using System.Collections.Generic;
using daemon_console.Models.ApiCalls;
using System.Linq;
using daemon_console.Models.Errors;
using System.Threading.Tasks;

namespace daemon_console
{
    class OtherMain
    {
        public static async void MainTester()
        {
            
            try
            {
                //string url = ApiCaller.GetSite();
                string url = ApiCaller.GetSite("?search=*");
                JObject apiResult = ApiCalls.GetGraphData(url).GetAwaiter().GetResult();
                //Console.WriteLine(apiResult.ToString());    
                SiteCall siteObject = JsonConvert.DeserializeObject<SiteCall>(apiResult.ToString());
                //Console.WriteLine(siteObject.value.First().siteId);
                if (siteObject != null)
                {
                    foreach (var site in siteObject.Value)
                    {
                        if (!site.WebUrl.Contains("/personal") && site.Name != null && site.WebUrl.Contains("ProjectFalcon-UXtest"))
                        {
                            Console.WriteLine(site.Name);
                            /*
                            url = $"sites/{site.siteId}/columns";
                            JObject result = ApiManager.RunAsync(url).GetAwaiter().GetResult();
                            Console.WriteLine(result.ToString());*/
                            string siteUrl = ApiCaller.GetDriveBySite(site.SiteId);
                            JObject driveResult = ApiCalls.GetGraphData(siteUrl).GetAwaiter().GetResult();
                            Console.WriteLine(site.Name);
                            Console.WriteLine(driveResult.ToString());  
                            if (driveResult != null)
                            {
                                Drive driveObject = JsonConvert.DeserializeObject<Drive>(driveResult.ToString());
                                Console.WriteLine(driveObject.Name);
                                string dirUrl = ApiCaller.GetFilesByDrive(driveObject.Id);
                                
                                JObject result = await ApiCalls.GetInsideDir(dirUrl, driveObject);
                                DirRoot dirObject = JsonConvert.DeserializeObject<DirRoot>(result.ToString());
                                Console.WriteLine(dirObject.Files[0].Name.ToString());
                                

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
        

       
        
    }
}
