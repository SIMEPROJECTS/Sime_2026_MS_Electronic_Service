using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Clients.Models;
using Newtonsoft.Json;




namespace MicroservicesEcosystem.Clients.Internal
{
    public class MSBusinessClient : Client, IMSBusinessClient
    {
        private const string uriMSWebApiService = "MS_Internal:CUriBusiness";

        public MSBusinessClient(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, ILogger<MSBusinessClient> _logger) : base(Configuration, httpContextAccessor, _logger, uriMSWebApiService)
        {
        }

        public async Task<BusinessRequestInfo> getBusinessInformation(Guid id)
        {
            var result = await MakeRequest(this.HttpClient, UrlClients.getBusinesinformation + "?id=" + id, HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<BusinessRequestInfo>(result);
        }

        //information


    }
}
