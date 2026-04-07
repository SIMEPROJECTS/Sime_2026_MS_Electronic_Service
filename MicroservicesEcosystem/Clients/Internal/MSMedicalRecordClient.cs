using MicroservicesEcosystem.Clients.Internal.Interface;
using MicroservicesEcosystem.Models.DTOs;
using Newtonsoft.Json;

namespace MicroservicesEcosystem.Clients.Internal
{
    public class MSMedicalRecordClient : Client, IMSMedicalRecordClient
    {
        private const string uriClient = "MS_Internal:CUriMedicalRecord";

        public MSMedicalRecordClient(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor, ILogger<MSMedicalRecordClient> _logger) : base(Configuration, httpContextAccessor, _logger, uriClient)
        {


        }

        public async Task PostTimeLogs(TimeLogsRequest timeLogsRequest)
        {
            var jsonData = JsonConvert.SerializeObject(timeLogsRequest, Formatting.Indented);
            try
            {
                await MakeRequestToken(this.HttpClient, UrlClients.postTimeLogs, HttpMethod.Post, jsonData);
            }
            catch (Exception ex)
            {
                //OJO Agregar logs de error 
            }
        }
    }
}
