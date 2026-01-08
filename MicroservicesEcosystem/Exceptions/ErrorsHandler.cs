using MicroservicesEcosystem.Response;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Net.Http;

namespace MicroservicesEcosystem.Exceptions
{
    public static class ErrorsHandler
    {
        public static Exception HandleBadRequest(HttpResponseMessage response)
        {
            _ = response;
            var message = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            switch (response.StatusCode)
            {
                case HttpStatusCode.ServiceUnavailable:
                    break;
                case HttpStatusCode.BadRequest:
                    var badMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var bad = JsonConvert.DeserializeObject<MsApiResponse>(badMessage);
                    message = bad.Message;
                    throw new ArgumentException(message);                    
                case HttpStatusCode.InternalServerError:
                    break;
                case HttpStatusCode.BadGateway:
                    break;
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.Forbidden:
                    var mess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();


                    try
                    {
                        var data = JsonConvert.DeserializeObject<ErrorsModel>(mess);
                        if (!string.IsNullOrEmpty(data.Cause))
                            message = data.Cause;
                        if (!string.IsNullOrEmpty(data.ErrorMessage))
                            message = data.ErrorMessage;
                    }
                    catch
                    {
                    }
                    break;
                case HttpStatusCode.Unauthorized:
                    mess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    message = Errors.ServiceUnauthorized;
                    try
                    {
                        var data = JsonConvert.DeserializeObject<ErrorsModel>(mess);
                        if (!string.IsNullOrEmpty(data.ErrorMessage))
                            message = data.ErrorMessage;
                    }
                    catch
                    {
                    }
                    break;
                case HttpStatusCode.GatewayTimeout:
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    break;
                case HttpStatusCode.RequestTimeout:
                    break;
                case HttpStatusCode.UnprocessableEntity:
                    break;
                default:
                    message = Errors.ServiceBadRequest;
                    break;
            }
            throw new RestException(message);
        }

        public static Exception HandleBadRequest(RestResponse response)
        {
            var message = response.Content ?? string.Empty;
            switch (response.StatusCode)
            {
                case HttpStatusCode.ServiceUnavailable:
                    break;
                case HttpStatusCode.BadRequest:
                    break;
                case HttpStatusCode.InternalServerError:
                    break;
                case HttpStatusCode.BadGateway:
                    break;
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.Forbidden:
                    try
                    {
                        var data = JsonConvert.DeserializeObject<ErrorsModel>(message);
                        if (!string.IsNullOrEmpty(data?.Cause))
                            message = data.Cause;
                        if (!string.IsNullOrEmpty(data?.ErrorMessage))
                            message = data.ErrorMessage;
                    }
                    catch { }
                    break;
                case HttpStatusCode.Unauthorized:
                    message = Errors.ServiceUnauthorized;
                    try
                    {
                        var data = JsonConvert.DeserializeObject<ErrorsModel>(message);
                        if (!string.IsNullOrEmpty(data?.ErrorMessage))
                            message = data.ErrorMessage;
                    }
                    catch { }
                    break;
                case HttpStatusCode.GatewayTimeout:
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    break;
                case HttpStatusCode.RequestTimeout:
                    break;
                case HttpStatusCode.UnprocessableEntity:
                    break;
                default:
                    message = Errors.ServiceBadRequest;
                    break;
            }

            throw new RestException(message);
        }

    }
}
