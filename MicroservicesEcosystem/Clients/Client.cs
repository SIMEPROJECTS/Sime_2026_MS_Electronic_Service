using MicroservicesEcosystem.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace MicroservicesEcosystem.Clients
{
    public class Client
    {
        public HttpClient HttpClient { get; }
        protected readonly IConfiguration Configuration;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected ILogger _logger;
        public Client(IConfiguration Configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger _logger,
            string uriClient)
        {
            this.Configuration = Configuration;
            this.httpContextAccessor = httpContextAccessor;
            this._logger = _logger;
            try
            {
                var envurl = this.Configuration[uriClient];
                // Verifica si la URL es válida
                if (!Uri.TryCreate(envurl, UriKind.Absolute, out Uri requestUri))
                {
                    throw new ArgumentException($"Invalid URL in configuration: {envurl}");
                }
                this.HttpClient = new HttpClient
                {
                    BaseAddress = requestUri
                };                

                this.HttpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (NullReferenceException e)
            {
                this._logger.LogWarning("Tenant could not be defined. Please set it manually with SetTenant function");
            }
        }
        public async Task<string> MakeRequestToken(HttpClient httpClient, string url, HttpMethod method, string data = null)
        {
            string json = string.Empty;

            try
            {
                // Obtener el token desde el contexto HTTP
                var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                                .ToString()
                                .Replace("Bearer ", string.Empty);

                if (method == HttpMethod.Get && data != null)
                {
                    var jObj = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(data);
                    var query = String.Join("&",
                                    jObj.Children().Cast<JProperty>()
                                    .Select(jp => jp.Name + "=" + HttpUtility.UrlEncode(jp.Value.ToString())));
                    url += "?" + query;
                }

                var request = new HttpRequestMessage(method, url);

                if (method != HttpMethod.Get && data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                // Agregar el encabezado Authorization si el token está disponible
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.SendAsync(request);
                _logger.LogInformation($"Requesting URL: {request.RequestUri}");

                json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    ErrorsHandler.HandleBadRequest(response);
                }
            }
            catch (OperationCanceledException)
            {
                throw new Exception(Errors.ServiceTimeout);
            }
            catch (HttpRequestException e)
            {
                throw new Exception(Errors.ServiceUnknowError);
            }
            catch (Exception e)
            {

                if (e is ArgumentException) throw;
                throw new Exception(e.Message);
            }

            return json;
        }

        public static String Authorization = "Authorization";
        public async Task<string> MakeRequest(HttpClient httpClient, string url, HttpMethod method, string data = null, string authorizationToken = null)
        {
            string json = string.Empty;
            try
            {
                if (method == HttpMethod.Get && data != null)
                {
                    var jObj = (JObject)JsonConvert.DeserializeObject(data);
                    var query = string.Join("&",
                                    jObj.Children().Cast<JProperty>()
                                    .Select(jp => jp.Name + "=" + HttpUtility.UrlEncode(jp.Value.ToString())));
                    url += "?" + query;
                }
                var request = new HttpRequestMessage(method, url)
                {
                    Content = (method == HttpMethod.Get) ? null : new StringContent(data, Encoding.UTF8, "application/json")
                };
                // Agregar encabezado de autorización
                if (!string.IsNullOrEmpty(authorizationToken))
                {
                    request.Headers.Add("Authorization", authorizationToken);
                }

                var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
                _logger.LogInformation($"Requesting URL: {request.RequestUri}");

                json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    ErrorsHandler.HandleBadRequest(response);
            }
            catch (OperationCanceledException)
            {
                throw new Exception(Errors.ServiceTimeout);
            }
            catch (HttpRequestException e)
            {
                throw new Exception(Errors.ServiceUnknowError);
            }
            catch (Exception e)
            {
                if (e is RestException) throw;
                throw new Exception(e.Message);
            }
            return json;
        }
    }
}
