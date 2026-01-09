using MicroservicesEcosystem.Clients.Models;
using Newtonsoft.Json;

namespace MicroservicesEcosystem.Clients.Internal.Interface
{
    public interface IMSBusinessClient
    {
        Task<BusinessRequestInfo> getBusinessInformation(Guid id);
    }
}

