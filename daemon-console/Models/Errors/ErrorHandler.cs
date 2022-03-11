using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_console.Models.Errors
{
    internal class ErrorHandler
    {
        public static JObject CreateNewError(string errorMessage)
        {
            RootError error = new RootError
            {
                Error = new Error
                {
                    Code = errorMessage,
                    Message = "Not good",
                    InnerError = new InnerError
                    {
                        RequestId = Guid.NewGuid(),
                        Date = DateTime.Now,
                        ClientRequestId = Guid.NewGuid(),
                        Code = errorMessage
                    }
                }
            };
            JObject errorJson = (JObject)JsonConvert.SerializeObject(error.ToString());
            return errorJson;
        }
    }
}
