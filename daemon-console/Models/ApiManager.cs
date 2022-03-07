﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates; //Only import this if you are using certificate
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;


namespace daemon_console.Models
{
    public class ApiManager
    {

        
        public static object ConvertToSiteObjects(JObject jsonObject)
        {
            if (jsonObject != null)
            {
                try
                {
                    if (jsonObject["value"].Count() > 1)
                    {
                        string stringedObject = jsonObject.ToString();
                        SiteCall SiteObject = JsonConvert.DeserializeObject<SiteCall>(stringedObject);
                        return SiteObject;
                    }
                    else if (jsonObject["value"].Count() == 1)
                    {
                        string stringedObject = jsonObject.ToString();
                        Site SiteObject = JsonConvert.DeserializeObject<Site>(stringedObject);
                        return SiteObject;
                    }
                }
                catch
                {
                    string stringedObject = jsonObject.ToString();
                    Error errorObject = JsonConvert.DeserializeObject<Error>(stringedObject);
                    return errorObject;
                }
                return null; 
            }
            else
            {
                return null;
            }
           
        }

        public static object ConvertToDriveObject(JObject jsonObject)
        {
            try
            {
                if (jsonObject != null)
                {
                    string stringedObject = jsonObject.ToString();
                    Drive driveObject = JsonConvert.DeserializeObject<Drive>(stringedObject);
                    return driveObject;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                string stringObject = jsonObject.ToString();
                Error error = JsonConvert.DeserializeObject<Error>(stringObject);
                return error;
            }
        }

        public static object ConvertToFile(JObject jsonObject)
        {
            try
            {
                if (jsonObject != null)
                {
                    string stringedObject = jsonObject.ToString();
                    DirRoot driveObject = JsonConvert.DeserializeObject<DirRoot>(stringedObject);
                    return driveObject;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                string stringObject = jsonObject.ToString();
                Error error = JsonConvert.DeserializeObject<Error>(stringObject);
                return error;
            }
        }

        public static async Task<JObject> RunAsync(string webUrl = null, bool callGraph = true, bool beta = false)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            string url = config.ApiUrl;
            Console.WriteLine(url);

            if (beta == false) 
            {
                url += "v1.0";
                Console.WriteLine(url);
            }
            else
            {
                url += "beta";
                Console.WriteLine(url);
            }


            if (webUrl == null)
            {
                url += $"/sites";
                Console.WriteLine(url);
            }
            else if (callGraph == true)
            {
                url += webUrl;
                Console.WriteLine(url);
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
                    Console.WriteLine(webUrl);
                    //await apiCaller.CallWebApiAndProcessResultASync($"{config.ApiUrl}v1.0/drives/b!m35i7pw9xk63vzHyWjqDzk_Pb0yQdL9KojjHhNPF3LPszlgMqS9gTLhxAfLg6bTB/root/children", result.AccessToken, Display);
                    Console.WriteLine(url);
                    object ApiResult = await apiCaller.CallWebApiAndProcessResultASync(url, result.AccessToken);
                    //Display(ApiResult);
                    Console.WriteLine("Hello");
                    try
                    {
                        if (ApiResult.GetType().Equals(string))
                        {
                            Console.WriteLine(ApiResult.ToString());
                        }
                        
                    }
                    catch
                    {
                        Console.WriteLine()
                    }
                    
                    
                    if (ApiResult.ContainsKey("error") && ApiResult != null)
                    {
                        RootError error = JsonConvert.DeserializeObject<RootError>(ApiResult.ToString());

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to call the web API: {error.Error.Message}");

                        // Note that if you got reponse.Code == 403 and reponse.content.code == "Authorization_RequestDenied"
                        // this is because the tenant admin as not granted consent for the application to call the Web API
                        Console.WriteLine($"Content: {error.Error.Message}");
                        Console.ResetColor();
                        return null;
                    }
                    return ApiResult;
                }
                catch
                {
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
