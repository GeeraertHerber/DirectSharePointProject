// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using daemon_console.Models;
using daemon_console.Models.OCR;
using System.Text;
using System.Collections.Generic;
using daemon_console.Models.Analytics;
using daemon_console.Models.Errors;

namespace daemon_console
{
    /// <summary>
    /// Helper class to call a protected API and process its result
    /// </summary>
    public class ProtectedApiCallHelper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient">HttpClient used to call the protected API</param>
        public ProtectedApiCallHelper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected HttpClient HttpClient { get; private set; }


        /// <summary>
        /// Calls the protected web API and processes the result
        /// </summary>
        /// <param name="webApiUrl">URL of the web API to call (supposed to return Json)</param>
        /// <param name="accessToken">Access token used as a bearer security token to call the web API</param>
        /// <param name="processResult">Callback used to process the result of the call to the web API</param>
        /// 

        public async Task<JObject> PostAnalyticsText(string[] content)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            string endpoint = config.TextAn1;
            string url = $"https://{endpoint}/text/analytics/v3.2-preview.2/analyze";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.OCRKey1}");
            string stringedContent = string.Join("", content);
            var analyticsObject = new AnalyticsRoot
            {
                DisplayName = "Extracting sentiment",
                AnalysisInput = {
                    Documents =
                    {
                        new Document
                        {
                            Id = "1",
                            Language = "en",
                            Text = stringedContent
                        }
                    }
                },
                Tasks = {
                    ExtractiveSummarizationTasks =
                    {
                        new ExtractiveSummarizationTask
                        {
                            Parameters = {
                                ModelVersion = "latest"
                            }
                        }
                    }
                }

            };
            Console.WriteLine(stringedContent);
            string jsonString = JsonConvert.SerializeObject(analyticsObject);
            //body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpContent httpContent = new StringContent(jsonString);
            HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
            Console.WriteLine(response.ToString());
            JObject json = JObject.Parse(jsonString);
            return json;
        }
        public async Task<JObject> PostOCRAsync(string url, byte[] byteArray)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.OCRKey1}");
            var body = new ByteArrayContent(byteArray);

            body.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await httpClient.PostAsync(url, body);
            List<JObject> ocrResponse = new List<JObject>();
            JObject ocrObject = new JObject();

            if ( response.Headers.TryGetValues("Operation-Location", out IEnumerable<string> responseUrl))
            {
                //Console.WriteLine(responseUrl.First());
                bool running = true;
                while (running)

                {
                    ocrResponse.Add(await CallCompletedOCRASync(responseUrl.First(), config));
                    if (!(ocrResponse[^1].GetValue("status").ToString() == "running"))
                    {
                        running = false;
                        break;
                    }
                    await Task.Delay(2000);
                }
                ocrObject = (JObject)JsonConvert.DeserializeObject(ocrResponse[^1].ToString());
            }
            
            return ocrObject;
                //httpClient..Add("Content-Type", "application/octet-stream");
        }

        public async Task<JObject> CallCompletedOCRASync(string responseurl, AuthenticationConfig config)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.OCRKey1}");
            var respons = await httpClient.GetAsync(responseurl);
            string json = await respons.Content.ReadAsStringAsync();
            
            JObject ocrRespons = JsonConvert.DeserializeObject(json) as JObject;
            //Console.WriteLine(ocrRespons.ToString());
            return ocrRespons;
        }

        public async Task<object> CallWebApiAndProcessResultASync(string webApiUrl, string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                var defaultRequestHeaders = HttpClient.DefaultRequestHeaders;
                if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
                defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await HttpClient.GetAsync(webApiUrl);

                HttpHeaders headers  = response.Content.Headers;
                //Console.WriteLine(headers.ToString());

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        if (!headers.ToString().Contains("application/pdf"))
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            //Console.WriteLine(headers.ToString().Contains("application/pdf"));

                            JObject result = JsonConvert.DeserializeObject(json) as JObject;
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.ResetColor();

                            return result;
                        }
                        else
                        {
                            
                            /// Console.WriteLine(child);
                            byte[] byteArray = await response.Content.ReadAsByteArrayAsync();
                            Console.WriteLine(byteArray);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.ResetColor();
                            return byteArray;
                        }
                    }
                    catch
                    {
                        JObject errorJson = ErrorHandler.CreateNewError("Real bad", "Conversion not succesfull");
                        return errorJson;
                    }
                    
                }
                else
                {
                    try
                    { 
                        string content = await response.Content.ReadAsStringAsync();
                        RootError error = JsonConvert.DeserializeObject<RootError>(content);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to call the web API: {error.Error.Message}");
                    

                        // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
                        // this is because the tenant admin as not granted consent for the application to call the Web API
                        Console.WriteLine($"Content: {error.Error.Message}");
                        Console.ResetColor();
                        return error;
                    }
                    catch
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        JObject errorJson = ErrorHandler.CreateNewError("Real bad", content);
                        return errorJson;
                    }
                }
               
            }
            else
            {
                return null;
            }
        }
    }
}
