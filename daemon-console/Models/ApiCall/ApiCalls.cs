using daemon_console.Models.Analytics;
using daemon_console.Models.Errors;
using daemon_console.Models.OCR;
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
        public static async Task<JObject> CallCompletedOCRASync(string responseurl)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.OCRKey1}");
            var respons = await httpClient.GetAsync(responseurl);
            string json = await respons.Content.ReadAsStringAsync();

            JObject ocrRespons = JsonConvert.DeserializeObject(json) as JObject;
            //Console.WriteLine(ocrRespons.ToString());
            return ocrRespons;
        }
        public static async Task<OCRResponse> PostOCRAsync(byte[] byteArray, string url = "")
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.OCRKey1}");
            var body = new ByteArrayContent(byteArray);

            body.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await httpClient.PostAsync(url, body);
            List<JObject> ocrResponse = new List<JObject>();
            OCRResponse ocrObject = new OCRResponse();

            if (response.Headers.TryGetValues("Operation-Location", out IEnumerable<string> responseUrl))
            {
                //Console.WriteLine(responseUrl.First());
                bool running = true;
                while (running)

                {
                    ocrResponse.Add(await CallCompletedOCRASync(responseUrl.First()));
                    if (!(ocrResponse[^1].GetValue("status").ToString() == "running"))
                    {
                       
                        break;
                    }
                    await Task.Delay(2000);
                }
                ocrObject = JsonConvert.DeserializeObject< OCRResponse>(ocrResponse[^1].ToString());
            }

            return ocrObject;
            //httpClient..Add("Content-Type", "application/octet-stream");
        }
        public static async Task<object> GetGraphData(string webUrl = null)
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
                    object apiResult = await apiCaller.CallWebApiAndProcessResultASync(webUrl, result.AccessToken);
                    Console.WriteLine(apiResult.GetType());
                    if (apiResult is Byte[])
                    {
                        //Console.WriteLine(encodedPDF);
                        return apiResult;
                    }
                    JObject JsonObject = JObject.Parse(apiResult.ToString());

                    if (JsonObject.ContainsKey("error") && JsonObject != null)
                    {
                        RootError error = JsonConvert.DeserializeObject<RootError>(JsonObject.ToString());

                        JObject errorObject = ErrorHandler.CreateNewError(error.Error.Code.ToString(), error.Error.Message.ToString());

                        Console.ForegroundColor = ConsoleColor.Red;
                        //Console.WriteLine($"Failed to call the web API: {error.Error.Message}");

                        // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
                        // this is because the tenant admin as not granted consent for the application to call the Web API
                        //Console.WriteLine($"Content: {error.Error.Message}");
                        Console.ResetColor();
                        return errorObject;
                    }
                    return JsonObject;


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    JObject error = ErrorHandler.CreateNewError("Real bad", "Error occured while calling for the graph API");
                    return error;
                }
            }
            throw new Exception("No apiresult came back");
        }
        public static async Task<HttpResponseMessage> PostAnalyticsText(string[] content)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            string endpoint = config.TextAnEndPoint;
            string url = $"{endpoint}text/analytics/v3.2-preview.2/analyze";

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{config.SPTextKey1}");
            string stringedContent = string.Join("", content);

            List<Document> documentList = new List<Document>();
            documentList.Add(
                        new Document
                        {
                            Id = "1",
                            Language = "en",
                            Text = stringedContent
                        }
                );

            List<ExtractiveSummarizationTask> extractTask = new List<ExtractiveSummarizationTask>();
            extractTask.Add(
                new ExtractiveSummarizationTask
                {
                    Parameters = new Parameters
                    {
                        ModelVersion = "Latest"
                    }
                });

            AnalyticsRoot analyticsObject = new AnalyticsRoot
            {
                DisplayName = "Extracting sentiment",
                AnalysisInput = new AnalysisInput
                {
                    Documents = documentList
                },
                Tasks = new Tasks
                {
                    ExtractiveSummarizationTasks = extractTask
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
        //public static async Task<object> GetWebAsync(string webApiUrl, string accessToken, HttpClient httpClient)
        //{
        //    if (!string.IsNullOrEmpty(accessToken))
        //    {
        //        var defaultRequestHeaders = httpClient.DefaultRequestHeaders;
        //        if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
        //        {
        //            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        }
        //        defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //        HttpResponseMessage response = await httpClient.GetAsync(webApiUrl);

        //        HttpHeaders headers = response.Content.Headers;
        //        //Console.WriteLine(headers.ToString());
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string json = await response.Content.ReadAsStringAsync();
        //            JObject result = JsonConvert.DeserializeObject(json) as JObject;
        //            return result;
        //        }
        //        else
        //        {
        //            JObject standardError = ErrorHandler.CreateNewError("Real bad", "Call not succesfull");
        //            return standardError;
        //        }

        //    }
        //    else
        //    {
        //        JObject standardError = ErrorHandler.CreateNewError("Real bad", "No accestoken provided");
        //        return standardError;
        //    }
        //}
        private static async Task<object> GetFile(Drive driveObject, FileSP fileObject)
        {
            object result = new object();
            Console.WriteLine(fileObject.Name);
            string[] fileFormats = new string[]
            {
                "csv", "doc", "docx", "odp", "ods", "odt", "pot", "potm", "potx", "pps", "ppsx", "ppsxm", "ppt", "pptm", "pptx", "rtf", "xls", "xlsx"
            };
            
            if (fileFormats.Any(fileObject.Name.Contains))
            {
                //string fileUrl = $"/drives/{driveObject.Id}/items/{fileObject.Id}?expand=fields"; //
                /*if (fileObject.Name.Contains(".pdf"))*/
                result = await ConvertPDF(driveObject, fileObject);
            }

            //else if (nonConvertable.Any(fileObject.Name.Contains))
            //{
            //    string fileUrl = $"/drives/{driveObject.Id}/items/{fileObject.Id}?expand=fields"; //
            //    /*if (fileObject.Name.Contains(".pdf"))*/
            //    result = await GetPDF(driveObject, fileObject);
            //}

            else if (fileObject.Name.Contains(".pdf"))
            {
                string url = ApiCaller.GetPDF(driveObject.Id, fileObject.Id);
                result = await GetGraphData(url);
            }
            else
            {
                result = ErrorHandler.CreateNewError("Real bad", "Not a supported documenttype");
            }
            return result;

        }
        public static async Task<JObject> GetInsideDir(string url, Drive driveObject)
        {
            JObject result = new JObject();
            object apiResult = await GetGraphData(url);
            DirRoot dirObject = JsonConvert.DeserializeObject<DirRoot>(apiResult.ToString());
            foreach (var dirContent in dirObject.Files)
            {
                if (dirContent.Folder != null)
                {
                    //Console.WriteLine(dirContent.Folder.ChildCount);
                    //Console.WriteLine(dirContent.WebUrl);
                    //string dirUrl = ApiCaller.GetFilesByDrive(driveObject.Id, "/schematics");
                    //result = await GetInsideDir(dirUrl, driveObject);
                    Console.WriteLine(dirContent.Name);
                }
            
                else if (dirContent.Folder == null)
                {
                    /* Console.WriteLine(dirContent.name);
                     Console.WriteLine(dirContent.eTag);*/
                    object fileResult = await GetFile(driveObject, dirContent);
                    if (fileResult is byte[] fileByteArray)
                    {
                        OCRResponse ocrResult = await PostOCRAsync(fileByteArray, "https://ocrdirecttester.cognitiveservices.azure.com/vision/v3.2/read/analyze?language=en");
                        string[] wordArray = ExtractWords(ocrResult);
                        result = await GetAnalytics(wordArray);
                        Console.WriteLine(result.ToString());
                    }
                }

            }
            return result;
        }

        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static async Task<byte[]> GetPDF(Drive driveObject, FileSP fileObject)
        {
            string url = ApiCaller.GetPDF(driveObject.Id, fileObject.Id);
            byte[] pdfFile = (byte[])await GetGraphData(url);
            return pdfFile;
        }
        private static async Task<byte[]> ConvertPDF(Drive driveObject, FileSP fileObject)
        {
            string[] parentPathArray = fileObject.ParentReference.Path.Split(":");
            Console.WriteLine(parentPathArray[1]);
            //string pdfUrl = $"/drives/{driveObject.Id}/root:/{fileObject.Name.Replace(" ", "%")}:/content?format=pdf";
            //
            string pdfUrl = ApiCaller.GetConvertFilePDF(driveObject.Id, fileObject.Name);
            Console.WriteLine(pdfUrl);
            
            byte[] apiResult = (byte[]) await GetGraphData(pdfUrl);
            //byte[] resultObject = JsonConvert.DeserializeObject<byte[]>(apiResult);
            return apiResult;

        }
        private static string[] ExtractWords(OCRResponse ocrObject)
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
            string stringedWordList = String.Join("", words);
            //JObject wordResult = JObject.Parse(stringedWordList);
            return words;


        }
        private static async Task<JObject> GetAnalytics(string[] wordArray)
        {
            Console.WriteLine(wordArray.Length);
            HttpResponseMessage httpResponse = await ApiCalls.PostAnalyticsText(wordArray);
            Console.WriteLine(httpResponse.StatusCode);
            return JObject.Parse(httpResponse.Content.ToString());
        }
        

    }
}
