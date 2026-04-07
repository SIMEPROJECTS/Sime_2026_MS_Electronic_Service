using MicroservicesEcosystem.Models.DTOs;

namespace MicroservicesEcosystem.Clients.Internal.Interface
{
    public interface IMSMedicalRecordClient
    {
        public Task PostTimeLogs(TimeLogsRequest timeLogsRequest);
    }
}
