using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using daemon_console.Models;
using daemon_console.Models.OCR;
using System.Collections.Generic;
using daemon_console.Models.ApiCalls;

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
                JObject apiResult = ApiCalls.GetGraphData(url).GetAwaiter().GetResult();
                SiteCall siteObject = JsonConvert.DeserializeObject<SiteCall>(apiResult.ToString());
                //Console.WriteLine(siteObject.value.First().siteId);
                if (siteObject != null)
                {
                    foreach (var site in siteObject.Value)
                    {
                        if (!site.WebUrl.Contains("/personal") && site.Name != null && site.WebUrl.Contains("Retail"))
                        {
                            Console.WriteLine(site.SiteId);
                            /*
                            url = $"sites/{site.siteId}/columns";
                            JObject result = ApiManager.RunAsync(url).GetAwaiter().GetResult();
                            Console.WriteLine(result.ToString());*/
                            string siteUrl = ApiCaller.GetDriveBySite(site.SiteId);
                            JObject driveResult = ApiCalls.GetGraphData(siteUrl).GetAwaiter().GetResult();
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
            JObject apiResult = ApiCalls.GetGraphData(url).GetAwaiter().GetResult();
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
            JObject apiResult = ApiCalls.GetGraphData(pdfUrl, true, true).GetAwaiter().GetResult();
            OCRResponse ocrObject = JsonConvert.DeserializeObject<OCRResponse>(apiResult.ToString());
            GetWords(ocrObject);
            
        }

        private static void GetWords(OCRResponse ocrObject)
        {
            List<string> wordList = new List<string>();
            //Console.WriteLine(wordList);
            foreach (ReadResult result in ocrObject.AnalyzeResult.ReadResults)
            {
                foreach (Line line in result.Lines)
                {
                    Console.WriteLine(line.Text);
                    foreach (var word in line.Text.Split(" "))
                    {
                        wordList.Add(word);
                    }
                    
                    /*foreach (string word in line.Text)
                    {
                        Console.WriteLine(word.Text);
                    }*/
                }
            }
            string[] words = wordList.ToArray();
            GetAnalytics(words);


        }
        private static async void GetAnalytics(string[] wordArray)
        {
            Console.WriteLine(wordArray.Length);
            HttpResponseMessage httpResponse = await ApiCalls.PostAnalyticsText(wordArray);
            Console.WriteLine(httpResponse.StatusCode);
        }
    }
}
