using MicroservicesEcosystem.CustomDataTime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MicroservicesEcosystem.Response
{
    public class ApiResponse
    {
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        public ApiResponse(Exception exception, HttpStatusCode code)
        {
            this.Timestamp = LocalDateTimeNow.Now().ToString();
            this.Status = code.GetHashCode();
            this.Error = exception.GetType().ToString();
            this.Message = exception.Message;
            this.Path = "";
        }

    }
    public class Errors
    {
        public string[] error { get; set; }

    }
    public class MsApiResponse
    {
        public string Timestamp { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
    }

}
