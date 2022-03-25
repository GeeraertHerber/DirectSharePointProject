using Newtonsoft.Json.Linq;
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
using daemon_console.Models.Analytics;
using System.Text;
using System.IO;

namespace daemon_console
{
    class OtherMain
    {
        public static async Task MainTesterAsync()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            try
            {
                string url = ApiCaller.GetSite("?search=*");
                object apiResult = ApiCalls.GetGraphData(url).GetAwaiter().GetResult();
                SiteCall siteObject = JsonConvert.DeserializeObject<SiteCall>(apiResult.ToString());
                if (siteObject != null)
                {
                    //List<string> siteNames = new List<string>();
                    foreach (var site in siteObject.Value)
                    {
                        if (!site.WebUrl.Contains("/personal") && site.Id != null)
                        {
                            Console.WriteLine(site.Name);
                            using StreamWriter siteNameFile = new StreamWriter("siteNames.csv", append: true);
                            await siteNameFile.WriteLineAsync($"{site.Name}");
                            string siteUrl = ApiCaller.GetDriveBySite(site.SiteId);
                            object driveResult = ApiCalls.GetGraphData(siteUrl).GetAwaiter().GetResult();
                            if (driveResult.ToString().Contains("@odata.context")) 

                            {
                                Drive driveObject = JsonConvert.DeserializeObject<Drive>(driveResult.ToString());
                                Console.WriteLine(driveObject.Name);
                                var fileUrl = ApiCaller.GetFilesByDrive(driveObject.Id);
                                List<Document> documentList = await ApiCalls.GetInsideDir(fileUrl.Item1, driveObject, fileUrl.Item2);
                                using StreamWriter file = new StreamWriter("textData.csv", append: true);

                                foreach (var doc in documentList)
                                {
                                    
                                    string newLine = $"{doc.Text}, {site.Name}";
                                    await file.WriteLineAsync(newLine);
                                }
                                //File.WriteAllText('C:\\textdata.csv', csv.ToString());
                                Console.WriteLine("Scanned a site");
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

            watch.Stop();
            Console.WriteLine($"Program finised in {watch.ElapsedMilliseconds/1000}s ");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }




    }
}
