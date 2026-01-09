using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Clients.Models;
using Newtonsoft.Json;
using System.Net.Mail;

namespace MicroservicesEcosystem.Clients.Internal
{
    public class MSMessagesClient : Client, IMSMessagesClient
    {
        private const string uriMSMessages = "MS_Internal:CUriMessages";
        public MSMessagesClient(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, ILogger<MSMessagesClient> _logger) : base(Configuration, httpContextAccessor, _logger, uriMSMessages)
        {


        }
        public async Task postEnvioOTPEmailPortalMessage(SendEmailMessageOTPRequest sendEmailCategory)
        {
            var jsonData = JsonConvert.SerializeObject(sendEmailCategory, Formatting.Indented);
            await MakeRequest(this.HttpClient, UrlClients.postEnvioOTPEmailMessagepPortal, HttpMethod.Post, jsonData);
        }
    }
}
