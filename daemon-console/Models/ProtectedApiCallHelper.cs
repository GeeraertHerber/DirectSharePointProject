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
                Console.WriteLine(headers.ToString());

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        if (!headers.ToString().Contains("application/pdf"))
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(headers.ToString().Contains("application/pdf"));

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
                else
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
               
            }
            else
            {
                return null;
            }
        }
    }
}
