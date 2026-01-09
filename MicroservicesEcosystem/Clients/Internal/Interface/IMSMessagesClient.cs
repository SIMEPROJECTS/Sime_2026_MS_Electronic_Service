using MicroservicesEcosystem.Clients.Models;

namespace MicroservicesEcosystem.Clients.Internal.Interface
{
    public interface IMSMessagesClient
    {
        Task postEnvioOTPSmsMessage(SendSmsMessageRequest sendEmailCategory);
        Task postEnvioOTPEmailPortalMessage(SendEmailMessageOTPRequest sendEmailCategory);
    }
}
