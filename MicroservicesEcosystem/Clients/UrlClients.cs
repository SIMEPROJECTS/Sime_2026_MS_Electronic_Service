using MicroservicesEcosystem.Models.DTO;

namespace MicroservicesEcosystem.Clients
{
    public class UrlClients
    {      

        public const string postEnvioOTPMessagePayment = "/api/messages/sms/otp/payment";
        public const string postEnvioOTPSmsMessage = "/api/messages/sms/otp";
        public const string postEnvioOTPEmailMessage = "/api/messages/email/otp";
        public const string postEnvioOTPEmailMessagepPortal = "/api/messages/email/otp/portal";
    }
}
