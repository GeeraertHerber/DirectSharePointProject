// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using daemon_console.Models.ApiCalls;

namespace daemon_console
{
    /// <summary>
    /// This sample shows how to query the Microsoft Graph from a daemon application
    /// which uses application permissions.
    /// For more information see https://aka.ms/msal-net-client-credentials
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool testing = false;
            if (testing == true)
            {
                try
                {
                    //string url = ApiCaller.GetSite();
                    string url = ApiCaller.GetSite("b!8gBvwHeRuECjw4izLaZWvHdN3S3rz75Kvhyrf0kHHx9SiVwv01P_Solc6sU6SAea");
                    JObject apiResult = (JObject)ApiCalls.GetGraphData(url).GetAwaiter().GetResult();
                    SiteCall siteObject = JsonConvert.DeserializeObject<SiteCall>(apiResult.ToString());
                    //Console.WriteLine(siteObject.value.First().siteId);
                    if (siteObject != null)
                    {
                        foreach (var site in siteObject.Value)
                        {
                            string siteUrl = ApiCaller.GetDriveBySite(site.SiteId);
                            JObject driveResult = (JObject)ApiCalls.GetGraphData(siteUrl).GetAwaiter().GetResult();
                            Console.WriteLine(site.Id);
                            if (driveResult != null)
                            {
                                //Drive conDrive = (Drive)ApiManager.ConvertToDriveObject(driveResult);
                                //string fileUrl = ApiCaller.GetFilesByDrive(conDrive.webUrl);
                                //JObject files = ApiManager.RunAsync(fileUrl).GetAwaiter().GetResult();
                                //Console.WriteLine(files.ToString());
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



            }
            else if (testing == false)
            {
                Console.WriteLine("Entering test program");
                daemon_console.OtherMain.MainTester();
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }        
    }
}
