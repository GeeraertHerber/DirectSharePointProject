﻿using daemon_console.Models.Analytics;
using daemon_console.Models.Errors;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace daemon_console.Models.ApiCalls
{
    public class ApiCalls
    {
        
        public ApiCalls(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected HttpClient HttpClient { get; private set; }
        public static bool AppUsesClientSecret(AuthenticationConfig config)
        {
            string clientSecretPlaceholderValue = "[Enter here a client secret for your application]";
            string certificatePlaceholderValue = "[Or instead of client secret: Enter here the name of a certificate (from the user cert store) as registered with your application]";

            if (!String.IsNullOrWhiteSpace(config.ClientSecret) && config.ClientSecret != clientSecretPlaceholderValue)
            {
                return true;
            }

            else if (!String.IsNullOrWhiteSpace(config.CertificateName) && config.CertificateName != certificatePlaceholderValue)
            {
                return false;
            }

            else
                throw new Exception("You must choose between using client secret or certificate. Please update appsettings.json file.");
        }

        public static X509Certificate2 ReadCertificate(string certificateName)
        {
            if (string.IsNullOrWhiteSpace(certificateName))
            {
                throw new ArgumentException("certificateName should not be empty. Please set the CertificateName setting in the appsettings.json", "certificateName");
            }
            CertificateDescription certificateDescription = CertificateDescription.FromStoreWithDistinguishedName(certificateName);
            DefaultCertificateLoader defaultCertificateLoader = new DefaultCertificateLoader();
            defaultCertificateLoader.LoadIfNeeded(certificateDescription);
            return certificateDescription.Certificate;
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

            if (response.Headers.TryGetValues("Operation-Location", out IEnumerable<string> responseUrl))
            {
                //Console.WriteLine(responseUrl.First());
                bool running = true;
                while (running)

                {
                    ocrResponse.Add(await CallCompletedOCRASync(responseUrl.First(), config));
                    if (!(ocrResponse[^1].GetValue("status").ToString() == "running"))
                    {
                       
                        break;
                    }
                    await Task.Delay(2000);
                }
                ocrObject = (JObject)JsonConvert.DeserializeObject(ocrResponse[^1].ToString());
            }

            return ocrObject;
            //httpClient..Add("Content-Type", "application/octet-stream");
        }
        public static async Task<JObject> GetGraphData(string webUrl = null, bool callGraph = true, bool beta = false)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            // You can run this sample using ClientSecret or Certificate. The code will differ only when instantiating the IConfidentialClientApplication
            bool isUsingClientSecret = AppUsesClientSecret(config);

            // Even if this is a console application here, a daemon application is a confidential client application
            IConfidentialClientApplication app;

            if (isUsingClientSecret)
            {
                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithClientSecret(config.ClientSecret)
                    .WithAuthority(new Uri(config.Authority))
                    .Build();
            }

            else
            {
                X509Certificate2 certificate = ReadCertificate(config.CertificateName);
                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithCertificate(certificate)
                    .WithAuthority(new Uri(config.Authority))
                    .Build();
            }

            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator. 
            string[] scopes = new string[] { $"{config.ApiUrl}.default" };

            AuthenticationResult result;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token acquired");
                Console.ResetColor();
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Scope provided is not supported");
                Console.ResetColor();
                throw new Exception("No token acquired");
            }

            if (result != null)
            {
                try
                {
                    var httpClient = new HttpClient();
                    var apiCaller = new ProtectedApiCallHelper(httpClient);
                    object ApiResult = await apiCaller.CallWebApiAndProcessResultASync(webUrl, result.AccessToken);
                    Console.WriteLine(ApiResult.GetType());
                    JObject JsonObject = JObject.Parse(ApiResult.ToString());

                    if (JsonObject.ContainsKey("error") && JsonObject != null)
                    {
                        RootError error = JsonConvert.DeserializeObject<RootError>(JsonObject.ToString());

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to call the web API: {error.Error.Message}");

                        // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
                        // this is because the tenant admin as not granted consent for the application to call the Web API
                        Console.WriteLine($"Content: {error.Error.Message}");
                        Console.ResetColor();
                        return null;
                    }
                    return JsonObject;


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    RootError error = new RootError
                    {
                        Error = new Error
                        {
                            Code = "Problem occured: empty string",
                            Message = "Not good",
                            InnerError = new InnerError
                            {
                                RequestId = Guid.NewGuid(),
                                Date = DateTime.Now,
                                ClientRequestId = Guid.NewGuid(),
                                Code = "Problem occured: empty string"
                            }
                        }
                    };
                    JObject errorJson = (JObject)JsonConvert.SerializeObject(error.ToString());
                    return errorJson;
                }
            }
            throw new Exception("No apiresult came back");
        }
        public static async Task<HttpResponseMessage> PostAnalyticsText(string[] content)
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
            return response;
        }

        public async Task<object> GetWebAsync(string webApiUrl, string accessToken)
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

                HttpHeaders headers = response.Content.Headers;
                //Console.WriteLine(headers.ToString());
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject result = JsonConvert.DeserializeObject(json) as JObject;
                    return result;
                }
                else
                {
                    JObject standardError = ErrorHandler.CreateNewError("Call not succesfull");
                    return standardError;
                }

            }
            else
            {
                JObject standardError = ErrorHandler.CreateNewError("No accestoken provided");
                return standardError;
            }
        }

    }
}
