using MicroservicesEcosystem.Clients.Models;
using MicroservicesEcosystem.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesEcosystem.Client.Internal.Interfaces
{
    public interface IMSIdentityClient
    {
        public Task postFile(FileUploadRequest fileUploadRequest);
    }
}
