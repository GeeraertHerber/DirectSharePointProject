using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
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
            else
            {
                return null;
            }
        }

        public static async Task<JObject> RunAsync(string parWebUrl = null)
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");

            string webURL = "";

            if (parWebUrl == null)
            {
                webURL = $"{config.ApiUrl}v1.0/sites";
            }
            else
            {
                webURL = $"{config.ApiUrl}v1.0/{parWebUrl}";
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

            AuthenticationResult result = null;
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
                var httpClient = new HttpClient();
                var apiCaller = new ProtectedApiCallHelper(httpClient);
                Console.WriteLine(webURL);
                //await apiCaller.CallWebApiAndProcessResultASync($"{config.ApiUrl}v1.0/drives/b!m35i7pw9xk63vzHyWjqDzk_Pb0yQdL9KojjHhNPF3LPszlgMqS9gTLhxAfLg6bTB/root/children", result.AccessToken, Display);
                JObject ApiResult = await apiCaller.CallWebApiAndProcessResultASync(webURL, result.AccessToken);
                //Display(ApiResult);
                return ApiResult;

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
