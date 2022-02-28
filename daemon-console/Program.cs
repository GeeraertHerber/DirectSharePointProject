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
            try
            {
                string Url = ApiCaller.GetSite();
                //string Url = URLCreator.GetFilesByDrive("b!8gBvwHeRuECjw4izLaZWvHdN3S3rz75Kvhyrf0kHHx9SiVwv01P_Solc6sU6SAea");
                JObject ApiResult = ApiManager.RunAsync(Url).GetAwaiter().GetResult();
                ApiManager.ConvertToSiteObjects(ApiResult);
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
