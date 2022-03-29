using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates; //Only import this if you are using certificate
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using daemon_console.Models.OCR;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace daemon_console.Models
{
    public class ApiManager
    {

        public static async Task<JObject> RunAsync(string webUrl = null, bool callGraph = true, bool beta = false)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            string url = config.ApiUrl;

            if (beta == false) 
            {
                url += "v1.0";
            }
            else
            {
                url += "beta";
            }


            if (webUrl == null)
            {
                url += $"/sites";
            }
            else if (callGraph == true)
            {
                url += webUrl;
            }



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
                    object ApiResult = await apiCaller.CallWebApiAndProcessResultASync(url, result.AccessToken);
                    Console.WriteLine(ApiResult.GetType());
                    if (ApiResult.GetType() == typeof(byte[]))
                    {
                        //await System.IO.File.WriteAllBytesAsync("hello_world.pdf", (byte[])ApiResult);
                        Console.WriteLine("Converting doc to PDF");
                        //JObject ocrRespons = await apiCaller.CallOCRApiASync("https://westeurope.api.cognitive.microsoft.com/vision/v3.2/read/analyze?language=en", (byte[])ApiResult);
                        Console.WriteLine("Done");
                        var ocrRespons = new JObject();


                        return ocrRespons;
                    }
                    else if (ApiResult.GetType() == typeof(JObject))
                    {
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
                    //Display(ApiResult);
                    
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

        /// <summary>
        /// Display the result of the Web API call
        /// </summary>
        /// <param name="result">Object to display</param>
        public static void Display(JObject result)
        {
            Console.WriteLine(result);
            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                Console.WriteLine($"{child.Name} = {child.Value}");
            }

            //string otherResult = result.ToString();
            //Models.Root myClass = JsonConvert.DeserializeObject<Models.Root>(otherResult);
            ////Console.WriteLine($"{myClass.OdataContext}");
            //foreach (var calledValue in myClass.value)
            //{
            //    Console.WriteLine(calledValue.name);
            //}
        }

        /// <summary>
        /// Checks if the sample is configured for using ClientSecret or Certificate. This method is just for the sake of this sample.
        /// You won't need this verification in your production application since you will be authenticating in AAD using one mechanism only.
        /// </summary>
        /// <param name="config">Configuration from appsettings.json</param>
        /// <returns></returns>
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
    }
}
