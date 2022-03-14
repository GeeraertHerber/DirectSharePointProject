using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models.Errors
{
    internal class ErrorHandler
    {
        public static JObject CreateNewError(string errorCode, string errorMessage)
        {
            RootError error = new RootError
            {
                Error = new Error
                {
                    Code = errorCode,
                    Message = errorMessage,
                    InnerError = new InnerError
                    {
                        RequestId = Guid.NewGuid(),
                        Date = DateTime.Now,
                        ClientRequestId = Guid.NewGuid(),
                        Code = errorMessage
                    }
                }
            };
            JObject errorJson = JObject.FromObject(error);
            Console.WriteLine(errorJson.ToString());
            return errorJson;
        }
    }
}
